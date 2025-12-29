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

public class WalletServiceTests
{
    private readonly Mock<IHoldingRepository> _holdingRepositoryMock;
    private readonly Mock<ITransferRepository> _transferRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly WalletService _walletService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Guid _testUserId = Guid.NewGuid();

    public WalletServiceTests()
    {
        _holdingRepositoryMock = new Mock<IHoldingRepository>();
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _walletService = new WalletService(
            _holdingRepositoryMock.Object,
            _transferRepositoryMock.Object,
            _walletRepositoryMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object
        );
        
        var claims = new List<Claim> 
        { 
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()) 
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task GetWalletSummaryAsync_Throws_WhenWalletNotFound()
    {
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(_testUserId)).ReturnsAsync((Wallet?)null);

        // Act
        Func<Task> act = async () => await _walletService.GetWalletSummaryAsync(1, 10);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Wallet not found");
    }

    [Fact]
    public async Task GetWalletSummaryAsync_ReturnsCorrectSummary()
    {
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 1000 };

        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(_testUserId)).ReturnsAsync(wallet);
        var transfersPaginatedDto = new TransfersPaginatedDto();
        _mapperMock.Setup(m => m.Map<WalletSummaryDto>(wallet)).Returns(new WalletSummaryDto{TransferPage=transfersPaginatedDto});

        var transfers = new Transfer[]
        {
            new Transfer { Id = Guid.NewGuid(), Amount = 100, Type = TransferType.Deposit, WalletId = wallet.Id }
        };
        var paginatedTransfers = new PaginatedResult<Transfer> { Items = transfers, CurrentPage = 1, TotalPages = 1 };
        _transferRepositoryMock.Setup(r => r.GetByWalletIdPaginatedAsync(wallet.Id, 1, 10)).ReturnsAsync(paginatedTransfers);
        _mapperMock.Setup(m => m.Map<List<TransferSummaryDto>>(transfers)).Returns(new List<TransferSummaryDto>
        {
            new TransferSummaryDto { Amount = 100, Type = TransferType.Deposit }
        });

        var stock = new Stock { Id = Guid.NewGuid(), StockName = "Tesla", Symbol= "TSLA", Description = "description of Tesla", PricePerShare = 10 };
        var holding = new Holding
        {
            Id = Guid.NewGuid(),
            StockId = stock.Id,
            WalletId = wallet.Id,
            Stock = stock,
            Orders = new List<Order>
            {
                new Order { Type = OrderType.Buy, Amount = 5, Total = 50 },
                new Order { Type = OrderType.Sell, Amount = 2, Total = 20 }
            }
        };
        _holdingRepositoryMock.Setup(r => r.GetByWalletIdAsync(wallet.Id)).ReturnsAsync(new List<Holding> { holding });
        _mapperMock.Setup(m => m.Map<List<HoldingSummaryDto>>(It.IsAny<List<Holding>>())).Returns(new List<HoldingSummaryDto> { new HoldingSummaryDto{StockName = "Tesla"} });

        // Act
        var result = await _walletService.GetWalletSummaryAsync(1, 10);

        // Assert
        result.Should().NotBeNull();
        result.TransferPage.Transfers.First().Amount.Should().Be(100);
        result.Holdings.First().Amount.Should().Be(3); // 5 buy - 2 sell
        result.Holdings.First().TotalValue.Should().Be(30); // 50 - 20
        result.Holdings.First().StockName.Should().Be("Tesla");
    }
}
