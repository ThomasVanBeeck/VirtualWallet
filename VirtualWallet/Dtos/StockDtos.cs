namespace VirtualWallet.Dtos;

public class StockDto
{
    public required string StockName { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public float PricePerShare { get; set; }
    public float ChangePct24Hr { get; set; }
}