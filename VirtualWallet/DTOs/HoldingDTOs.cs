namespace VirtualWallet.DTOs;

public class HoldingDTO
{
    public string StockName { get; set; }
    public List<OrderDTO> Orders { get; set; }
}

public class HoldingSummaryDTO
{
    public string StockName { get; set; }
    public float Amount { get; set; }
    public float CurrentPrice { get; set; }
    public float TotalValue { get; set; }
    public float TotalProfit { get; set; }
    public float WinLossPct { get; set; }
}