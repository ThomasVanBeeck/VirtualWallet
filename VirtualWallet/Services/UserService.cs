
using Microsoft.AspNetCore.Identity;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
/*
        public async Task<UserDto?> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync();
            if (user == null) return null;

            return new UserDto
            {
                Username = $"{user.Voornaam} {user.Achternaam}"
            };
        }*/

        public async Task<UserDto?> GetCurrentUserAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return new UserDto
            {
                Username = user.FirstName +  " " + user.LastName
            };
        }
    }

    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
    }
}