using VirtualWallet.Enums;

namespace VirtualWallet.DTOs;

public class OrderDTO
{
    public string StockName { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
    public float Total  { get; set; }
}

public class OrderPostDTO
{
    public string StockName { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
}