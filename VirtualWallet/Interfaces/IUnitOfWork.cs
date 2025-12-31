namespace VirtualWallet.Interfaces;

public interface IUnitOfWork: IDisposable
{
    Task SaveChangesAsync();
}