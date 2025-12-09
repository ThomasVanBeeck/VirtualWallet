namespace VirtualWallet.Dtos;

public class HoldingSummaryDto
{
    public required string StockName { get; set; }
    public float Amount { get; set; }
    public float CurrentPrice { get; set; }
    public float TotalValue { get; set; }
    public float TotalProfit { get; set; }
    public float WinLossPct { get; set; }
}