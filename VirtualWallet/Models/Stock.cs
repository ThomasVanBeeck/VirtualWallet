using VirtualWallet.Enums;

namespace VirtualWallet.Models;

public class Stock
{
    public Guid Id { get; set; }
    public string StockName { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public StockType Type { get; set; }
    public string Description { get; set; } = null!;
    public float PricePerShare { get; set; }
    public float ChangePct24Hr { get; set; }
    public List<Holding> Holdings { get; set; } = new();
}