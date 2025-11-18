namespace VirtualWallet.DTOs;

public class TransferDTO
{
    public string Type { get; set; } = null!;
    public float Amount { get; set; }
    public DateTime OrderDate { get; set; }
}