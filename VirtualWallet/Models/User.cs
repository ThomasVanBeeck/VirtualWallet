
namespace VirtualWallet.Models;

    public class User
    {
        public Guid Id { get; set; }
        
        //one to one
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;
        
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }