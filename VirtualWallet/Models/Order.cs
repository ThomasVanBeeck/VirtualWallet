using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Order
{
    public Guid Id { get; set; }
    
    // many to one
    public Guid HoldingId { get; set; }
    public required Holding Holding { get; set; }
    public DateTime Date { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
    public float Total { get; set; }
}