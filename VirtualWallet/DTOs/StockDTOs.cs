namespace VirtualWallet.DTOs;

public class StockDTO
{
    public string StockName { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public float PricePerShare { get; set; }
    public float ChangePct24Hr { get; set; }
}