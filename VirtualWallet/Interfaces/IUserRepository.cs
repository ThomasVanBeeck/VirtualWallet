using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    void AddAsync(User user);
}