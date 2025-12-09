using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Transfer
{
    public Guid Id { get; set; }
    public TransferType Type {get; set;}
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    
    // many to one
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
}