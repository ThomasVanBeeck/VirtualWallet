
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using VirtualWallet.DTOs;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(UserRepository userRepository,  IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            var passwordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordCorrect) return null;

            return user;
        }

        public async Task<UserDTO?> GetCurrentUserAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return _mapper.Map<UserDTO>(user);
        }
    }
    
}