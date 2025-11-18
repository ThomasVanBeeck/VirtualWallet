using VirtualWallet.Enums;

namespace VirtualWallet.DTOs;

public class OrderDTO
{
    public DateTime OrderDate { get; set; }
    public OrderType Type { get; set; }
    public float Price { get; set; }
    public float Amount { get; set; }
}