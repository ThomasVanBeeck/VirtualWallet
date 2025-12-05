using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Transfer
{
    public Guid Id { get; set; }
    public TransferType Type {get; set;}
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
}