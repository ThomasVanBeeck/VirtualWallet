using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using VirtualWallet.Dtos;
using VirtualWallet.Enums;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;
using VirtualWallet.Services;

namespace VirtualWallet.Tests.ServiceTests;

public class TransferServiceTests
{
    private readonly Mock<ITransferRepository> _transferRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TransferService _transferService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Guid _testUserId = Guid.NewGuid();

    public TransferServiceTests()
    {
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        var claims = new List<Claim> 
        { 
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()) 
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);        

        _transferService = new TransferService(
            _transferRepositoryMock.Object,
            _walletRepositoryMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task AddTransferAsync_Throws_WhenWalletNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Wallet?)null);
        var transferDto = new TransferDto { Type = TransferType.Deposit, Amount = 100 };

        // Act
        Func<Task> act = async () => await _transferService.AddTransferAsync(transferDto);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Wallet not found for user");
    }

    [Fact]
    public async Task AddTransferAsync_Throws_WhenWithdrawalInsufficientFunds()
    {
        // Arrange
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 50 };
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(_testUserId)).ReturnsAsync(wallet);

        var transferDto = new TransferDto { Type = TransferType.Withdrawal, Amount = 100 };

        // Act
        Func<Task> act = async () => await _transferService.AddTransferAsync(transferDto);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Insufficient funds available to withdrawal.");
    }

    [Fact]
    public async Task AddTransferAsync_AddsDepositSuccessfully()
    {
        // Arrange
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 100 };
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(_testUserId)).ReturnsAsync(wallet);

        var transferDto = new TransferDto { Type = TransferType.Deposit, Amount = 50 };
        var transferEntity = new Transfer();
        _mapperMock.Setup(m => m.Map<Transfer>(transferDto)).Returns(transferEntity);

        // Act
        await _transferService.AddTransferAsync(transferDto);

        // Assert
        _transferRepositoryMock.Verify(r => r.AddAsync(It.Is<Transfer>(t => t.WalletId == wallet.Id)), Times.Once);
        _walletRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Wallet>(w => w.TotalCash == 150)), Times.Once);
    }

    [Fact]
    public async Task AddTransferAsync_AddsWithdrawalSuccessfully()
    {
        // Arrange
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 200 };
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(_testUserId)).ReturnsAsync(wallet);

        var transferDto = new TransferDto { Type = TransferType.Withdrawal, Amount = 50 };
        var transferEntity = new Transfer();
        _mapperMock.Setup(m => m.Map<Transfer>(transferDto)).Returns(transferEntity);

        // Act
        await _transferService.AddTransferAsync(transferDto);

        // Assert
        _transferRepositoryMock.Verify(r => r.AddAsync(It.Is<Transfer>(t => t.WalletId == wallet.Id)), Times.Once);
        _walletRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Wallet>(w => w.TotalCash == 150)), Times.Once);
    }
}
