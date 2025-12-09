using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Stock
{
    public Guid Id { get; set; }
    public required string StockName { get; set; }
    public required string Symbol { get; set; }
    public StockType Type { get; set; }
    public required string Description { get; set; }
    public float PricePerShare { get; set; }
    public float ChangePct24Hr { get; set; }
    public List<Holding> Holdings { get; set; } = new();
}