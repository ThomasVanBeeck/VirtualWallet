using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Enums;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class TransferService
{
    private readonly TransferRepository _transferRepository;
    private readonly WalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public TransferService(TransferRepository transferRepository, WalletRepository walletRepository, IMapper mapper)
    {
        _transferRepository = transferRepository;
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    public async Task AddTransferAsync(Guid userId, TransferDTO transferDTO)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
            throw new Exception("Wallet not found for user");
        
        if (transferDTO.Type == TransferType.Withdrawal && transferDTO.Amount > wallet.TotalCash)
            throw new Exception("Insufficient funds available to withdrawal.");
        
        var entity = _mapper.Map<Transfer>(transferDTO);
        
        entity.Id = Guid.NewGuid();
        entity.WalletId = wallet.Id;
        
        await _transferRepository.AddAsync(entity);
        
        switch (transferDTO.Type)
        {
            case TransferType.Deposit:
                wallet.TotalCash += transferDTO.Amount;
                break;
            case TransferType.Withdrawal:
                wallet.TotalCash -= transferDTO.Amount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // TODO hier nog TotalProfit en WinLossPct updaten
        // adhv alle holdings wordt dit opnieuw berekend
        await _walletRepository.UpdateAsync(wallet);
    }
}