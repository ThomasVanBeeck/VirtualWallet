using System.Net;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using VirtualWallet.Dtos;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;
using VirtualWallet.Services;

namespace VirtualWallet.Tests.ServiceTests;

public class StockServiceTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IScheduleTimerRepository> _scheduleTimerRepositoryMock;
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly HttpClient _httpClient;
    private readonly StockService _stockService;

    public StockServiceTests()
    {
        _stockRepositoryMock = new Mock<IStockRepository>();
        _mapperMock = new Mock<IMapper>();
        _scheduleTimerRepositoryMock = new Mock<IScheduleTimerRepository>();
        _settingsServiceMock = new Mock<ISettingsService>();
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["ApiKeys:AlphaVantage"]).Returns("FAKE_KEY");

        // Mock HttpClient met HttpMessageHandler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"TimeSeriesDaily\":{\"2025-12-09\":{\"High\":\"100\",\"Low\":\"90\"},\"2025-12-08\":{\"High\":\"95\",\"Low\":\"85\"}}}")
            });

        _httpClient = new HttpClient(handlerMock.Object);

        _stockService = new StockService(
            _stockRepositoryMock.Object,
            _mapperMock.Object,
            _configMock.Object,
            _httpClient,
            _scheduleTimerRepositoryMock.Object,
            _settingsServiceMock.Object
        );
    }

    [Fact]
    public async Task GetAllStocks_ReturnsMappedStocks()
    {
        // Arrange
        var stocks = new List<Stock>
        {
            new Stock { Id = Guid.NewGuid(), Symbol = "TSLA", StockName = "Tesla", Description = "description of Tesla"}
        };
        _stockRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stocks);
        var stockDtos = new List<StockDto> { new StockDto { StockName = "Tesla", Type = "Crypto", Description = "description of Tesla"} };
        _mapperMock.Setup(m => m.Map<List<StockDto>>(stocks)).Returns(stockDtos);

        // Act
        var result = await _stockService.GetAllStocks();

        // Assert
        result.Should().BeEquivalentTo(stockDtos);
    }

    [Fact]
    public async Task UpdateStockPrices_DoesNotUpdate_WhenIntervalNotElapsed()
    {
        // Arrange
        _settingsServiceMock.Setup(s => s.GetLastUpdateTimestampAsync()).ReturnsAsync(DateTime.UtcNow);

        // Act
        await _stockService.UpdateStockPrices();

        // Assert
        _stockRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Stock>()), Times.Never);
        _settingsServiceMock.Verify(s => s.SetLastUpdateTimestampAsync(It.IsAny<DateTime>()), Times.Never);
    }
}
