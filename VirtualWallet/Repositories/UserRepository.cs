using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories
{
    public class UserRepository: AbstractBaseRepository<User>, IUserRepository

    {
        public UserRepository(AppDbContext context): base(context)
        { }
        
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await DbSet.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}