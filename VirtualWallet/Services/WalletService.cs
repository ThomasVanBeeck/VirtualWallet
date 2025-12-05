using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class WalletService
{
    private readonly TransferRepository _transferRepository;
    private readonly WalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletService(TransferRepository transferRepository, WalletRepository walletRepository, IMapper mapper)
    {
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
        // hier moet ook nog de nieuwe TotalProfit en WinLossPct geupdated worden
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
        return  walletSummaryDTO;
    }

    public async Task AddWalletAsync(Guid userId)
    {
        var wallet = new Wallet()
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
        await _walletRepository.AddAsync(wallet);
    }

    public async Task UpdateWalletAsync(Wallet wallet)
    {
        // hier nog TotalProfit en WinLossPct updaten
        // adhv alle holdings wordt dit opnieuw berekend
        await _walletRepository.UpdateAsync(wallet);
    }
}

    