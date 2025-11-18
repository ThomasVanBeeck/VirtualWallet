namespace VirtualWallet.Models;

public class Holding
{
    public Guid Id { get; set; }
    
    // many to one
    public Guid WalletId { get; set; }
    public Wallet Wallet { get; set; } = null!;
    
    // many to one
    public Guid StockId { get; set; }
    public Stock Stock { get; set; } = null!;

    public List<Order> Orders { get; set; } = new();
}