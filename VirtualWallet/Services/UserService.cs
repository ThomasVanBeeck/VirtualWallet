
using AutoMapper;
using VirtualWallet.Dtos;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IWalletRepository walletRepository, IMapper mapper)
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

    public async Task<UserDto?> GetCurrentUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> GetUsernameExists(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return false;
        else return true;
    }

    public async Task CreateUserAsync(UserRegisterDto userRegisterDto)
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