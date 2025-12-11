using System.Globalization;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Services;

public class ScheduleTimerService : ISettingsService
{
    private const string TimestampKey = "LastStockUpdateTimestamp";
    private readonly IScheduleTimerRepository _scheduleTimerRepository;

    public ScheduleTimerService(IScheduleTimerRepository scheduleTimerRepository)
    {
        _scheduleTimerRepository = scheduleTimerRepository;
    }

    public async Task<DateTime> GetLastUpdateTimestampAsync()
    {
        var scheduleTimer = await _scheduleTimerRepository.GetAsync(TimestampKey);

        if (scheduleTimer == null)
            // Als het nog niet bestaat, oudst mogelijke datum instellen
            return DateTime.MinValue;
        if (DateTime.TryParse(scheduleTimer.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out var lastRunTime)) return lastRunTime;

        // Fallback in geval waarde corrupt is
        return DateTime.MinValue;
    }

    public async Task SetLastUpdateTimestampAsync(DateTime timestamp)
    {
        var scheduleTimer = await _scheduleTimerRepository.GetAsync(TimestampKey);

        var timestampUtc = timestamp.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);

        if (scheduleTimer == null)
            scheduleTimer = new ScheduleTimer
            {
                Key = TimestampKey,
                Value = timestampUtc
            };
        else
            scheduleTimer.Value = timestampUtc;

        await _scheduleTimerRepository.UpdateOrAddAsync(scheduleTimer);
    }
}