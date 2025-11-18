namespace VirtualWallet.Services;

using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;
using VirtualWallet.Repositories;
using System.Globalization; // Nodig voor InvariantCulture

public class ScheduleTimerService : ISettingsService
{
    private readonly ScheduleTimerRepository _scheduleTimerRepository;
    private const string TimestampKey = "LastStockUpdateTimestamp";

    public ScheduleTimerService(ScheduleTimerRepository scheduleTimerRepository)
    {
        _scheduleTimerRepository = scheduleTimerRepository;
    }

    public async Task<DateTime> GetLastUpdateTimestampAsync()
    {
        var scheduleTimer = await _scheduleTimerRepository.GetAsync(TimestampKey);

        if (scheduleTimer == null)
        {
            // If it doesn't exist yet, returns oldest date as value
            return DateTime.MinValue; 
        }
        if (DateTime.TryParse(scheduleTimer.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var lastRunTime))
        {
            return lastRunTime.ToUniversalTime(); 
        }

        // fallback in case value is corrupt
        return DateTime.MinValue; 
    }

    public async Task SetLastUpdateTimestampAsync(DateTime timestamp)
    {
        var scheduleTimer = await _scheduleTimerRepository.GetAsync(TimestampKey);
        
        string timestampUtc = timestamp.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);

        if (scheduleTimer == null)
        {
            scheduleTimer = new ScheduleTimer
            {
                Key = TimestampKey,
                Value = timestampUtc
            };
        }
        else
        {
            scheduleTimer.Value = timestampUtc;
        }

        await _scheduleTimerRepository.UpdateOrAddAsync(scheduleTimer);
    }
}