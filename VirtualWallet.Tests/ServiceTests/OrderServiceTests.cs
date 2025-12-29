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

public class OrderServiceTests
{
    private readonly Mock<IHoldingRepository> _holdingRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IWalletRepository> _walletRepositoryMock;
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly OrderService _orderService;
    private readonly Guid _testUserId = Guid.NewGuid();

    public OrderServiceTests()
    {
        _holdingRepositoryMock = new Mock<IHoldingRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _stockRepositoryMock = new Mock<IStockRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _mapperMock = new Mock<IMapper>();
        
        var claims = new List<Claim> 
        { 
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()) 
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        _orderService = new OrderService(
            _holdingRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _walletRepositoryMock.Object,
            _stockRepositoryMock.Object,
            _httpContextAccessorMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task AddOrderAsync_Throws_WhenWalletNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Wallet?)null);
        var dto = new OrderPostDto { StockName = "AAPL", Type = OrderType.Buy, Amount = 10, Price = 100 };

        // Act
        Func<Task> act = async () => await _orderService.AddOrderAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Wallet not found for user");
    }

    [Fact]
    public async Task AddOrderAsync_Throws_WhenStockNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 1000 };
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(wallet);
        _stockRepositoryMock.Setup(r => r.GetByNameAsync("AAPL")).ReturnsAsync((Stock?)null);

        var dto = new OrderPostDto { StockName = "AAPL", Type = OrderType.Buy, Amount = 10, Price = 100 };

        // Act
        Func<Task> act = async () => await _orderService.AddOrderAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Stock not found");
    }

    [Fact]
    public async Task AddOrderAsync_Throws_WhenInsufficientFunds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 500 };
        var stock = new Stock { Id = Guid.NewGuid(), StockName = "Tesla", Symbol = "TSLA", Description = "description of Tesla" };
        var holding = new Holding { Id = Guid.NewGuid(), StockId = stock.Id, WalletId = wallet.Id, Orders = new List<Order>() };

        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(wallet);
        _stockRepositoryMock.Setup(r => r.GetByNameAsync("Tesla")).ReturnsAsync(stock);
        _holdingRepositoryMock.Setup(r => r.GetByWalletAndStockAsync(stock.Id, wallet.Id)).ReturnsAsync(holding);

        var dto = new OrderPostDto { StockName = "Tesla", Type = OrderType.Buy, Amount = 10, Price = 100 };

        // Act
        Func<Task> act = async () => await _orderService.AddOrderAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Insufficient funds");
    }


    [Fact]
    public async Task AddOrderAsync_AddsBuyOrderSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wallet = new Wallet { Id = Guid.NewGuid(), TotalCash = 2000, TotalInStocks = 0 };
        var stock = new Stock { Id = Guid.NewGuid(),
            StockName = "Tesla", Symbol="TSLA", Description = "description of Tesla"};

        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(wallet);
        _stockRepositoryMock.Setup(r => r.GetByNameAsync("AAPL")).ReturnsAsync(stock);
        _holdingRepositoryMock.Setup(r => r.GetByWalletAndStockAsync(stock.Id, wallet.Id)).ReturnsAsync((Holding?)null);

        var holding = new Holding { Id = Guid.NewGuid(), StockId = stock.Id, WalletId = wallet.Id };
        _holdingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Holding>())).ReturnsAsync(holding);

        var orderEntity = new Order();
        _mapperMock.Setup(m => m.Map<Order>(It.IsAny<OrderPostDto>())).Returns(orderEntity);

        var dto = new OrderPostDto { StockName = "AAPL", Type = OrderType.Buy, Amount = 10, Price = 100 };

        // Act
        await _orderService.AddOrderAsync(dto);

        // Assert
        _orderRepositoryMock.Verify(r => r.AddAsync(It.Is<Order>(o => o.Total == 1000 && o.HoldingId == holding.Id && o.WalletId == wallet.Id)), Times.Once);
        _walletRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Wallet>(w => w.TotalCash == 1000 && w.TotalInStocks == 1000)), Times.Once);
    }

    [Fact]
    public async Task GetOrdersAsync_Throws_WhenWalletNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((Wallet?)null);

        // Act
        Func<Task> act = async () => await _orderService.GetOrdersAsync(1, 10);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Wallet not found");
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsMappedOrders()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wallet = new Wallet { Id = Guid.NewGuid() };
        _walletRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(wallet);

        var orders = new Order[] { new Order { Id = Guid.NewGuid() } };
        var paginatedOrders = new PaginatedResult<Order> { Items = orders, CurrentPage = 1, TotalPages = 1 };
        _orderRepositoryMock.Setup(r => r.GetOrdersByWalletIdAsync(wallet.Id, 1, 10)).ReturnsAsync(paginatedOrders);

        var ordersDto = new List<OrderDto> { new OrderDto { StockName = "Tesla"} };
        _mapperMock.Setup(m => m.Map<List<OrderDto>>(orders)).Returns(ordersDto);

        // Act
        var result = await _orderService.GetOrdersAsync(1, 10);

        // Assert
        result.Orders.Should().BeEquivalentTo(ordersDto);
        result.PageNumber.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }
}
