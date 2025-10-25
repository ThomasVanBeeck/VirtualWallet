using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories

{
    public class WalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        // Alle wallets ophalen
        public async Task<List<Wallet>> GetAllAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        // Wallet ophalen op ID
        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await _context.Wallets.FindAsync(id);
        }

        // Nieuwe wallet toevoegen
        public async Task AddAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        // Wallet updaten
        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        // Wallet verwijderen
        public async Task DeleteAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
                await _context.SaveChangesAsync();
            }
        }
    }
}