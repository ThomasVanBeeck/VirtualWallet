using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Enums;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class WalletService
{
    private readonly HoldingRepository _holdingRepository;
    private readonly TransferRepository _transferRepository;
    private readonly WalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletService(HoldingRepository holdingRepository, TransferRepository transferRepository, WalletRepository walletRepository, IMapper mapper)
    {
        _holdingRepository = holdingRepository;
        _transferRepository = transferRepository;
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    public Task<Wallet?> GetWalletByUserIdAsync(Guid userId)
    {
        return _walletRepository.GetByUserIdAsync(userId);
    }

    public async Task<WalletSummaryDTO> GetWalletSummaryAsync(Guid userId, int page, int size)
    {
        // TODO hier moet ook nog de nieuwe TotalProfit en WinLossPct geupdated worden
        // adhv alle holdings berekenen
        
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        if (wallet == null)
            throw new Exception("Wallet not found");
        
        var walletSummaryDTO = _mapper.Map<WalletSummaryDTO>(wallet);
        var transfersPaginatedResult = await _transferRepository.GetByWalletIdPaginatedAsync(wallet.Id, page, size);
        var itemsDTO = _mapper.Map<List<TransferSummaryDTO>>(transfersPaginatedResult.Items);
        var transferPage = new TransfersPaginatedDTO
        {
            Transfers = itemsDTO,
            PageNumber = transfersPaginatedResult.CurrentPage,
            TotalPages = transfersPaginatedResult.TotalPages
        };
        walletSummaryDTO.TransferPage = transferPage;

        var holdings = await _holdingRepository.GetByWalletIdAsync(wallet.Id);
        var holdingsSummaryDTO = _mapper.Map<List<HoldingSummaryDTO>>(holdings);
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
            
            holdingsSummaryDTO[i].StockName = holdings[i].Stock.StockName;
            holdingsSummaryDTO[i].Amount = amount;
            holdingsSummaryDTO[i].CurrentPrice = currentPrice;
            holdingsSummaryDTO[i].TotalValue = totalValue;
            if (amount == 0.0f)
            {
                holdingsSummaryDTO[i].TotalProfit = 0.0f;
                holdingsSummaryDTO[i].WinLossPct = 0.0f;
            }
            else
            {
                holdingsSummaryDTO[i].TotalProfit = totalValue - (amount * currentPrice);
                holdingsSummaryDTO[i].WinLossPct = totalValue / (amount * currentPrice) - 1.0f;
            }
        }
        walletSummaryDTO.Holdings = holdingsSummaryDTO;
        return  walletSummaryDTO;
    }
}