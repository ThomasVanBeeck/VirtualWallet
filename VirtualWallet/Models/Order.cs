using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Order
{
    public Guid Id { get; set; }
    
    // many to one
    public Guid HoldingId { get; set; }
    public Holding Holding { get; set; } = null!;
    
    // many to one
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    
    public DateTime Date { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
    public float Total { get; set; }
}