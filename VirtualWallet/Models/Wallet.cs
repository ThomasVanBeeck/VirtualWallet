namespace VirtualWallet.Models;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<Holding> Holdings { get; set; } = new List<Holding>();
}