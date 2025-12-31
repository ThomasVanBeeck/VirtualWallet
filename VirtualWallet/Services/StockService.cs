using System.Globalization;
using System.Text.Json;
using AutoMapper;
using VirtualWallet.Dtos;
using VirtualWallet.Enums;
using VirtualWallet.Interfaces;
using VirtualWallet.Models.ExternalAPIs;

namespace VirtualWallet.Services;

public class StockService: AbstractBaseService
{
    private readonly string _apiKey;
    private readonly string _apiUrlCrypto;
    private readonly string _apiUrlStock;
    private readonly HttpClient _http;
    private readonly IMapper _mapper;
    private readonly IScheduleTimerRepository _scheduleTimerRepository;
    private readonly ISettingsService _settingsService;
    private readonly IStockRepository _stockRepository;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(24);
    private const string TimestampKey = "LastStockUpdateTimestamp";

    public StockService(IStockRepository stockRepository,
        IMapper mapper,
        IConfiguration config,
        HttpClient http,
        IScheduleTimerRepository scheduleTimerRepository
        ,ISettingsService settingsService,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, unitOfWork)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _http = http;
        _scheduleTimerRepository =  scheduleTimerRepository;
        _settingsService = settingsService;
        _apiUrlStock = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY";
        _apiUrlCrypto = "https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&market=USD";
        _apiKey = config["ApiKeys:AlphaVantage"];
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("ApiKeys:AlphaVantage is not configured.");
    }

    public async Task<List<StockDto>> GetAllStocks()
    {
        var stocks = await _stockRepository.GetAllAsync();
        return _mapper.Map<List<StockDto>>(stocks);
    }

    public async Task UpdateStockPrices()
    {
        var lastUpdate = await _settingsService.GetLastUpdateTimestampAsync();
        var timeSinceLastUpdate = DateTime.UtcNow - lastUpdate;
        Console.WriteLine($"Last time checked for updating stock prices: {timeSinceLastUpdate.ToString()}");
        Console.WriteLine($"Update interval: {_updateInterval.ToString()}");
        Console.WriteLine($"Last update: {(timeSinceLastUpdate - _updateInterval).TotalHours}");

        if (timeSinceLastUpdate > _updateInterval)
        {
            var stocks = await _stockRepository.GetAllAsync();
            {
                foreach (var t in stocks)
                {
                    var symbol = t.Symbol;
                    var stockType = t.Type;
                    try
                    {
                        Console.WriteLine($"Updating stock price of {symbol}...");
                        var queryUri = "";
                        if (stockType == StockType.Stock)
                            queryUri = $"{_apiUrlStock}&symbol={symbol}&apikey={_apiKey}";
                        else
                            queryUri = $"{_apiUrlCrypto}&symbol={symbol}&apikey={_apiKey}";
                        var jsonString = await _http.GetStringAsync(queryUri);
                        if (jsonString == "") Console.WriteLine($"Failed to get json string of {symbol}.");
                        var stock = JsonSerializer.Deserialize<StockAlphaVantage>(jsonString)!;
                        //Console.WriteLine($"json string of symbol {symbol}: {jsonString}.");

                        var timeSeries = stock.TimeSeriesDaily;
                        var sortedDailyQuotes = timeSeries
                            .OrderByDescending(kvp => DateTime.Parse(kvp.Key, CultureInfo.InvariantCulture))
                            .ToList();

                        if (sortedDailyQuotes.Count == 0)
                        {
                            Console.WriteLine($"Failed to get external stock price of symbol {symbol}.");
                            continue;
                        }

                        var latestDailyQuote = sortedDailyQuotes.FirstOrDefault().Value;
                        var previousDailyQuote = sortedDailyQuotes.Skip(1).First().Value;

                        var latestHigh = float.Parse(latestDailyQuote.High, CultureInfo.InvariantCulture);
                        var latestLow = float.Parse(latestDailyQuote.Low, CultureInfo.InvariantCulture);
                        var previousHigh = float.Parse(previousDailyQuote.High, CultureInfo.InvariantCulture);
                        var previousLow = float.Parse(previousDailyQuote.Low, CultureInfo.InvariantCulture);

                        var dayPrice = (latestHigh + latestLow) / 2f;
                        var previousDayPrice = (previousHigh + previousLow) / 2f;

                        var changePct24Hr = 100f * (dayPrice / previousDayPrice - 1f);

                        var currentStock = t;
                        currentStock.ChangePct24Hr = changePct24Hr;
                        currentStock.PricePerShare = dayPrice;
                        _stockRepository.UpdateAsync(currentStock);
                        await UnitOfWork.SaveChangesAsync();
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"HTTP Exception of symbol {symbol}: {ex.Message}");
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Conversion Exception of symbol {symbol}: {ex.Message}");
                    }
                }
            }
            await _settingsService.SetLastUpdateTimestampAsync(DateTime.UtcNow);
        }
    }

    public async Task<StockUpdateDto> GetLastUpdateTimestamp()
    {
        const string displayFormat = "yyyy-MM-dd HH:mm:ss"; 
        
        var scheduleTimer = await _scheduleTimerRepository.GetByTimestampKeyAsync(TimestampKey);
    
        DateTime lastRunTimeUtc;
        
        if (scheduleTimer == null)
        {
            lastRunTimeUtc = DateTime.MinValue;
        }
        else if (DateTime.TryParse(
                     scheduleTimer.Value, 
                     CultureInfo.InvariantCulture, 
                     DateTimeStyles.RoundtripKind,
                     out var parsedTime)
                )
        {
            lastRunTimeUtc = parsedTime; 
        }
        else
        {
            lastRunTimeUtc = DateTime.MinValue;
        }
        
        var stockUpdateDto = new StockUpdateDto
        {
            LastUpdate = lastRunTimeUtc.ToString(displayFormat, CultureInfo.InvariantCulture)
        };
    
        return stockUpdateDto;
    }
}