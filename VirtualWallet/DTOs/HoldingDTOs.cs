namespace VirtualWallet.DTOs;

public class HoldingDTO
{
    public string StockName { get; set; }
    public List<OrderDTO> Orders { get; set; }
}

public class HoldingSummaryDTO
{
    public string StockName { get; set; }
}