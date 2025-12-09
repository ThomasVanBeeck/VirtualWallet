using AutoMapper;
using VirtualWallet.Dtos;
using VirtualWallet.Enums;
using VirtualWallet.Interfaces;

namespace VirtualWallet.Services;

public class WalletService
{
    private readonly IHoldingRepository _holdingRepository;
    private readonly ITransferRepository _transferRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletService(IHoldingRepository holdingRepository, ITransferRepository transferRepository, IWalletRepository walletRepository, IMapper mapper)
    {
        _holdingRepository = holdingRepository;
        _transferRepository = transferRepository;
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    public async Task<WalletSummaryDto> GetWalletSummaryAsync(Guid userId, int page, int size)
    {
        // TODO hier moet ook nog de nieuwe TotalProfit en WinLossPct geupdated worden
        // adhv alle holdings berekenen
        
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            throw new Exception("Wallet not found");
        
        var walletSummaryDto = _mapper.Map<WalletSummaryDto>(wallet);
        var transfersPaginatedResult = await _transferRepository.GetByWalletIdPaginatedAsync(wallet.Id, page, size);
        var itemsDto = _mapper.Map<List<TransferSummaryDto>>(transfersPaginatedResult.Items);
        var transferPage = new TransfersPaginatedDto
        {
            Transfers = itemsDto,
            PageNumber = transfersPaginatedResult.CurrentPage,
            TotalPages = transfersPaginatedResult.TotalPages
        };
        walletSummaryDto.TransferPage = transferPage;

        var holdings = await _holdingRepository.GetByWalletIdAsync(wallet.Id);
        var holdingsSummaryDto = _mapper.Map<List<HoldingSummaryDto>>(holdings);
        for (int i = 0; i < holdings.Count(); i++)
        {
            var buys = holdings[i].Orders.Where(o => o.Type == OrderType.Buy);
            var sells = holdings[i].Orders.Where(o => o.Type == OrderType.Sell);
            
            var totalBought = buys.Sum(b => b.Amount);
            var totalSold =  sells.Sum(s => s.Amount);
            var amount = totalBought - totalSold;

            var totalValueBought = buys.Sum(b => b.Total);
            var totalValueSold = sells.Sum(s => s.Total);
            var totalValue = totalValueBought - totalValueSold;
            
            var currentPrice = holdings[i].Stock.PricePerShare;
            
            holdingsSummaryDto[i].StockName = holdings[i].Stock.StockName;
            holdingsSummaryDto[i].Amount = amount;
            holdingsSummaryDto[i].CurrentPrice = currentPrice;
            holdingsSummaryDto[i].TotalValue = totalValue;
            if (amount == 0.0f)
            {
                holdingsSummaryDto[i].TotalProfit = 0.0f;
                holdingsSummaryDto[i].WinLossPct = 0.0f;
            }
            else
            {
                holdingsSummaryDto[i].TotalProfit = totalValue - (amount * currentPrice);
                holdingsSummaryDto[i].WinLossPct = totalValue / (amount * currentPrice) - 1.0f;
            }
        }
        walletSummaryDto.Holdings = holdingsSummaryDto;
        return  walletSummaryDto;
    }
}