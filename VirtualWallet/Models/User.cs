
namespace VirtualWallet.Models;

    public class User
    {
        public Guid Id { get; set; }
        
        //one to one
        public Wallet Wallet { get; set; } = null!;
        
        public required string Username { get; set; } = null!;
        public required string FirstName { get; set; } = null!;
        public required string LastName { get; set; } = null!;
        public required string Email { get; set; } = null!;
        public required string PasswordHash { get; set; } = null!;
    }