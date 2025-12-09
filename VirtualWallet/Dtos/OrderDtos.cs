using VirtualWallet.Enums;

namespace VirtualWallet.Dtos;

public class OrderDto
{
    public required string StockName { get; set; }
    public DateTime Date { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
    public float Total  { get; set; }
}

public class OrderPostDto
{
    public required string StockName { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
}

public class OrdersPaginatedDto
{
    public List<OrderDto> Orders { get; set; } = new();
    public float PageNumber { get; set; }
    public float TotalPages { get; set; }
}