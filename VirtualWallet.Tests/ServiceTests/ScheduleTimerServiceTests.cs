using FluentAssertions;
using Moq;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;
using VirtualWallet.Services;

namespace VirtualWallet.Tests.ServiceTests;

public class ScheduleTimerServiceTests
{
    private readonly Mock<IScheduleTimerRepository> _scheduleTimerRepositoryMock;
    private readonly ScheduleTimerService _service;

    public ScheduleTimerServiceTests()
    {
        _scheduleTimerRepositoryMock = new Mock<IScheduleTimerRepository>();
        _service = new ScheduleTimerService(_scheduleTimerRepositoryMock.Object);
    }

    [Fact]
    public async Task GetLastUpdateTimestampAsync_ReturnsMinValue_WhenNoEntryExists()
    {
        // Arrange
        _scheduleTimerRepositoryMock.Setup(r => r.GetAsync("LastStockUpdateTimestamp"))
            .ReturnsAsync((ScheduleTimer?)null);

        // Act
        var result = await _service.GetLastUpdateTimestampAsync();

        // Assert
        result.Should().Be(DateTime.MinValue);
    }


    [Fact]
    public async Task GetLastUpdateTimestampAsync_ReturnsMinValue_WhenCorruptEntryExists()
    {
        // Arrange
        var storedTimer = new ScheduleTimer
        {
            Key = "LastStockUpdateTimestamp",
            Value = "not-a-date"
        };
        _scheduleTimerRepositoryMock.Setup(r => r.GetAsync("LastStockUpdateTimestamp"))
            .ReturnsAsync(storedTimer);

        // Act
        var result = await _service.GetLastUpdateTimestampAsync();

        // Assert
        result.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public async Task SetLastUpdateTimestampAsync_AddsNewEntry_WhenNoneExists()
    {
        // Arrange
        _scheduleTimerRepositoryMock.Setup(r => r.GetAsync("LastStockUpdateTimestamp"))
            .ReturnsAsync((ScheduleTimer?)null);

        var newTimestamp = DateTime.UtcNow;

        // Act
        await _service.SetLastUpdateTimestampAsync(newTimestamp);

        // Assert
        _scheduleTimerRepositoryMock.Verify(r => r.UpdateOrAddAsync(
            It.Is<ScheduleTimer>(s => s.Key == "LastStockUpdateTimestamp" &&
                                       s.Value == newTimestamp.ToString("o"))), Times.Once);
    }

    [Fact]
    public async Task SetLastUpdateTimestampAsync_UpdatesExistingEntry_WhenExists()
    {
        // Arrange
        var existingTimer = new ScheduleTimer
        {
            Key = "LastStockUpdateTimestamp",
            Value = DateTime.UtcNow.AddDays(-1).ToString("o")
        };
        _scheduleTimerRepositoryMock.Setup(r => r.GetAsync("LastStockUpdateTimestamp"))
            .ReturnsAsync(existingTimer);

        var newTimestamp = DateTime.UtcNow;

        // Act
        await _service.SetLastUpdateTimestampAsync(newTimestamp);

        // Assert
        _scheduleTimerRepositoryMock.Verify(r => r.UpdateOrAddAsync(
            It.Is<ScheduleTimer>(s => s == existingTimer && s.Value == newTimestamp.ToString("o"))), Times.Once);
    }
}
