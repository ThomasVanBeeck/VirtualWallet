
using AutoMapper;
using VirtualWallet.DTOs;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

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

    public async Task<UserDTO?> CreateUserAsync(UserRegisterDTO userRegisterDto)
    {
        if (await _userRepository.GetByUsernameAsync(userRegisterDto.Username) != null)
        {
            throw new Exception("Username already exists");
        }
        var user = _mapper.Map<User>(userRegisterDto);
        var newUser = await _userRepository.AddAsync(user);
        return _mapper.Map<UserDTO>(newUser);
    }
}