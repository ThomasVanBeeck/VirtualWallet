using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories

{
    public class WalletRepository: AbstractBaseRepository<Wallet>, IWalletRepository
    {

        public WalletRepository(AppDbContext context): base(context)
        { }

        public async Task<Wallet?> GetByUserIdAsync(Guid userId)
        {
            return await DbSet.FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}