using AutoMapper;
using VirtualWallet.Dtos;
using VirtualWallet.Enums;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Services;

public class TransferService
{
    private readonly ITransferRepository _transferRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public TransferService(ITransferRepository transferRepository, IWalletRepository walletRepository, IMapper mapper)
    {
        _transferRepository = transferRepository;
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    public async Task AddTransferAsync(Guid userId, TransferDto transferDto)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        
        if (wallet == null)
            throw new Exception("Wallet not found for user");
        
        if (transferDto.Type == TransferType.Withdrawal && transferDto.Amount > wallet.TotalCash)
            throw new Exception("Insufficient funds available to withdrawal.");
        
        var entity = _mapper.Map<Transfer>(transferDto);
        
        entity.Id = Guid.NewGuid();
        entity.WalletId = wallet.Id;
        
        await _transferRepository.AddAsync(entity);
        
        switch (transferDto.Type)
        {
            case TransferType.Deposit:
                wallet.TotalCash += transferDto.Amount;
                break;
            case TransferType.Withdrawal:
                wallet.TotalCash -= transferDto.Amount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // TODO hier nog TotalProfit en WinLossPct updaten
        // adhv alle holdings wordt dit opnieuw berekend
        await _walletRepository.UpdateAsync(wallet);
    }
}