using AutoMapper;
using VirtualWallet.Dtos;
using VirtualWallet.Enums;
using VirtualWallet.Interfaces;

namespace VirtualWallet.Services;

public class WalletService: AbstractService
{
    private readonly IHoldingRepository _holdingRepository;
    private readonly ITransferRepository _transferRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletService(IHoldingRepository holdingRepository, ITransferRepository transferRepository, IWalletRepository walletRepository, IMapper mapper,
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _holdingRepository = holdingRepository;
        _transferRepository = transferRepository;
        _walletRepository = walletRepository;
        _mapper = mapper;
    }
    
    public class Lot
    {
        public float Amount { get; set; }
        public float PricePerShare { get; set; }
    }
    
    public async Task<WalletSummaryDto> GetWalletSummaryAsync(int page, int size)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(UserId);
        if (wallet == null)
            throw new Exception("Wallet not found");

        var walletSummaryDto = _mapper.Map<WalletSummaryDto>(wallet);

        var transfersPaginatedResult = await _transferRepository.GetByWalletIdPaginatedAsync(wallet.Id, page, size);
        var itemsDto = _mapper.Map<List<TransferSummaryDto>>(transfersPaginatedResult.Items);
        walletSummaryDto.TransferPage = new TransfersPaginatedDto
        {
            Transfers = itemsDto,
            PageNumber = transfersPaginatedResult.CurrentPage,
            TotalPages = transfersPaginatedResult.TotalPages
        };

        var holdings = await _holdingRepository.GetByWalletIdAsync(wallet.Id);
        var holdingSummaryDtoList = _mapper.Map<List<HoldingSummaryDto>>(holdings);

        for (int i = 0; i < holdings.Count; i++)
        {
            var orders = holdings[i].Orders
                .OrderBy(o => o.Date)
                .ToList();

            var lots = new Queue<Lot>();

            foreach (var order in orders)
            {
                if (order.Type == OrderType.Buy)
                {
                    lots.Enqueue(new Lot
                    {
                        Amount = order.Amount,
                        PricePerShare = order.Total / order.Amount
                    });
                }
                else if (order.Type == OrderType.Sell)
                {
                    var amountToSell = order.Amount;
                    while (amountToSell > 0 && lots.Count > 0)
                    {
                        var lot = lots.Peek();
                        if (lot.Amount <= amountToSell)
                        {
                            amountToSell -= lot.Amount;
                            lots.Dequeue();
                        }
                        else
                        {
                            lot.Amount -= amountToSell;
                            amountToSell = 0;
                        }
                    }
                }
            }

            var remainingAmount = lots.Sum(l => l.Amount);
            var totalCost = lots.Sum(l => l.Amount * l.PricePerShare);
            var currentPrice = holdings[i].Stock.PricePerShare;

            holdingSummaryDtoList[i].StockName = holdings[i].Stock.StockName;
            holdingSummaryDtoList[i].Amount = remainingAmount;
            holdingSummaryDtoList[i].CurrentPrice = currentPrice;
            holdingSummaryDtoList[i].TotalValue = remainingAmount * currentPrice;

            if (remainingAmount <= 0 || totalCost <= 0)
            {
                holdingSummaryDtoList[i].TotalProfit = 0f;
                holdingSummaryDtoList[i].WinLossPct = 0f;
            }
            else
            {
                holdingSummaryDtoList[i].TotalProfit = (currentPrice * remainingAmount) - totalCost;
                holdingSummaryDtoList[i].WinLossPct = (holdingSummaryDtoList[i].TotalProfit / totalCost) * 100f;
            }
        }

        walletSummaryDto.Holdings = holdingSummaryDtoList;

        walletSummaryDto.TotalProfit = holdingSummaryDtoList.Sum(h => h.TotalProfit);
        walletSummaryDto.TotalInStocks = holdingSummaryDtoList.Sum(h => h.TotalValue);
        
        var totalValue = walletSummaryDto.TotalInStocks;
        if (totalValue > 0)
        {
            var weightedWinLoss = holdingSummaryDtoList
                .Sum(h => (h.TotalValue / totalValue) * h.WinLossPct);
            walletSummaryDto.WinLossPct = weightedWinLoss;
        }
        else
        {
            walletSummaryDto.WinLossPct = 0f;
        }
        return walletSummaryDto;
    }
}