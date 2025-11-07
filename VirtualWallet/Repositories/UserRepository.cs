using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
/*
        public async Task ResetAndSeedUsersAsync()
        {
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();

            var users = new List<User>
            {
                new User { Id = Guid.Empty, Voornaam = "Default", Achternaam = "User", Woonplaats = "Antwerpen" },
                new User { Id = Guid.NewGuid(), Voornaam = "Thomas", Achternaam = "Peeters", Woonplaats = "Brussel" },
                new User { Id = Guid.NewGuid(), Voornaam = "Marie", Achternaam = "Janssens", Woonplaats = "Gent" },
                new User { Id = Guid.NewGuid(), Voornaam = "Lucas", Achternaam = "De Vries", Woonplaats = "Leuven" },
                new User { Id = Guid.NewGuid(), Voornaam = "Sara", Achternaam = "Claes", Woonplaats = "Mechelen" }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }
    */
    }
}