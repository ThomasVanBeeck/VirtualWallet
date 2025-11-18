using System.Globalization;
using System.Text.Json;
using VirtualWallet.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using VirtualWallet.DTOs;
using VirtualWallet.Enums;
using VirtualWallet.Models;
using VirtualWallet.Models.ExternalAPIs;

namespace VirtualWallet.Services;

public class StockService
{
    private readonly StockRepository _stockRepository;
    private readonly IMapper _mapper;
    private readonly string _apiKey;
    private readonly string _apiUrlStock;
    private readonly string _apiUrlCrypto;
    private readonly HttpClient _http;
    private readonly ISettingsService _settingsService;
    private readonly TimeSpan _updateInterval = TimeSpan.FromHours(24);

    public StockService(StockRepository stockRepository, IMapper mapper, IConfiguration config, HttpClient http, ISettingsService settingsService)
    {
        _stockRepository = stockRepository;
        _mapper = mapper;
        _http = http;
        _settingsService = settingsService;
        _apiUrlStock = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY";
        _apiUrlCrypto = "https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&market=USD";
        _apiKey = config["ApiKeys:AlphaVantage"];
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("ApiKeys:AlphaVantage is not configured.");
        }
    }

    public async Task<List<StockDTO>> GetAllStocks()
    {
        var stocks = await _stockRepository.GetAllAsync();
        return _mapper.Map<List<StockDTO>>(stocks);
    }

    public async Task UpdateStockPrices()
    {
        DateTime lastUpdate = await _settingsService.GetLastUpdateTimestampAsync();
        TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastUpdate;
        Console.WriteLine($"Last time checked for updating stock prices: {timeSinceLastUpdate.ToString()}");
        Console.WriteLine($"Update interval: {_updateInterval.ToString()}");
        Console.WriteLine($"Last update: {(timeSinceLastUpdate - _updateInterval).TotalHours}");
        
        if (timeSinceLastUpdate > _updateInterval)
        {
            List<Stock> stocks = await _stockRepository.GetAllAsync();
            {
                for (int i = 0; i < stocks.Count; i++)
                {
                    string symbol = stocks[i].Symbol;
                    StockType stockType = stocks[i].Type;
                    try
                    {
                        Console.WriteLine($"Updating stock price of {symbol}...");
                        string queryUri = "";
                        if(stockType == StockType.Stock)
                            queryUri = $"{_apiUrlStock}&symbol={symbol}&apikey={_apiKey}";
                        else
                            queryUri = $"{_apiUrlCrypto}&symbol={symbol}&apikey={_apiKey}";
                        string jsonString = await _http.GetStringAsync(queryUri);
                        if (jsonString == "")
                        {
                            Console.WriteLine($"Failed to get json string of {symbol}.");
                        }
                        StockAlphaVantage stock = JsonSerializer.Deserialize<StockAlphaVantage>(jsonString)!;
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
                        
                        DailyQuoteAlphaVantage latestDailyQuote = sortedDailyQuotes.FirstOrDefault().Value;
                        DailyQuoteAlphaVantage previousDailyQuote = sortedDailyQuotes.Skip(1).First().Value;
                        
                        float latestHigh = float.Parse(latestDailyQuote.High, CultureInfo.InvariantCulture);
                        float latestLow = float.Parse(latestDailyQuote.Low, CultureInfo.InvariantCulture);
                        float previousHigh = float.Parse(previousDailyQuote.High, CultureInfo.InvariantCulture);
                        float previousLow = float.Parse(previousDailyQuote.Low, CultureInfo.InvariantCulture);

                        float dayPrice = (latestHigh + latestLow) / 2f;
                        float previousDayPrice = (previousHigh + previousLow) / 2f;
                        
                        float changePct24Hr = 100f * (dayPrice/previousDayPrice - 1f);

                        Stock currentStock = stocks[i];
                        currentStock.ChangePct24Hr = changePct24Hr;
                        currentStock.PricePerShare = dayPrice;
                        await _stockRepository.UpdateAsync(currentStock);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"HTTP Exception of symbol {symbol}: {ex.Message}");
                        continue;
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Conversion Exception of symbol {symbol}: {ex.Message}");
                        continue;
                    }
                }
            }
            await _settingsService.SetLastUpdateTimestampAsync(DateTime.UtcNow);
        }
    }
}