namespace VirtualWallet.Models;

public class Wallet
{
    public Guid Id { get; set; }
    
    // one to one, wallet is dependent
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Holding> Holdings { get; set; } = new();
    public List<Transfer> Transfers { get; set; } = new();
}