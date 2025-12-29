using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using VirtualWallet.Dtos;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;
using VirtualWallet.Services;

namespace VirtualWallet.Tests.ServiceTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly UserService _userService;

    private readonly User _mockUser;
    private readonly UserDto _mockUserDto;
    private readonly UserRegisterDto _mockUserRegisterDto;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Guid _testUserId = Guid.NewGuid();

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        
        var claims = new List<Claim> 
        { 
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()) 
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        _userService = new UserService(
            _userRepositoryMock.Object,
            _walletRepositoryMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object
        );

        _mockUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "johnnyboy1995",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct1234!"),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };
        
        _mockUserDto = new UserDto
        {
            Username = "johnnyboy1995",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };

        _mockUserRegisterDto = new UserRegisterDto
        {
            Username = "johnnyboy1995",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Password = "Correct1234!"
        };

    }
    
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var username = "johnnyboy1995";
        var password = "Correct1234!";
        

        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(_mockUser);

        // Act
        var result = await _userService.LoginAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be(username);
    }
    
    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        // Arrange

        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync("john"))
            .ReturnsAsync(_mockUser);

        // Act
        var result = await _userService.LoginAsync("john", "wrong");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentUserAsync_UserExists_ReturnsUserDto()
    {
        
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(_testUserId))
            .ReturnsAsync(_mockUser);

        _mapperMock
            .Setup(m => m.Map<UserDto>(_mockUser))
            .Returns(_mockUserDto);

        // Act
        var result = await _userService.GetCurrentUserAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("johnnyboy1995");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Doe");
        result!.Email.Should().Be("john@test.com");
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetUsernameExists_ReturnsExpectedResult(bool userExists)
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync("johnnyboy1995"))
            .ReturnsAsync(userExists ? _mockUser : null);

        // Act
        var result = await _userService.GetUsernameExists("johnnyboy1995");

        // Assert
        result.Should().Be(userExists);
    }
    
    [Fact]
    public async Task CreateUserAsync_NewUser_AddsUserAndWallet()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByUsernameAsync(_mockUserRegisterDto.Username))
            .ReturnsAsync((User?)null);

        _mapperMock
            .Setup(m => m.Map<User>(_mockUserRegisterDto))
            .Returns(_mockUser);

        // Act
        await _userService.CreateUserAsync(_mockUserRegisterDto);

        // Assert
        _userRepositoryMock.Verify(
            r => r.AddAsync(_mockUser),
            Times.Once
        );

        _walletRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Wallet>(w => w.UserId == _mockUser.Id)),
            Times.Once
        );
    }
}
