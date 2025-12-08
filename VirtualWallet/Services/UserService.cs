
using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly WalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public UserService(UserRepository userRepository, WalletRepository walletRepository,  IMapper mapper)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
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

    public async Task<UserDTO?> GetCurrentUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<bool> GetUsernameExists(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return false;
        else return true;
    }

    public async Task CreateUserAsync(UserRegisterDTO userRegisterDto)
    {
        if (await _userRepository.GetByUsernameAsync(userRegisterDto.Username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }
        var user = _mapper.Map<User>(userRegisterDto);
        await _userRepository.AddAsync(user);
        var wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = user.Id
        };
        await _walletRepository.AddAsync(wallet);
    }
}