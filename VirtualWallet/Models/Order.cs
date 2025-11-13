using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Order
{
    public Guid Id { get; set; }
    
    // many to one 
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    // many to one
    public Guid StockId { get; set; }
    public Stock Stock { get; set; }
    
    // many to one
    public Guid HoldingId { get; set; }
    public Holding Holding { get; set; }
    
    public DateTime OrderDate { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
}