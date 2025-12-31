using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IScheduleTimerRepository
{
    Task<ScheduleTimer?> GetByTimestampKeyAsync(string timestampKey);
    Task UpdateOrAddAsync(ScheduleTimer timer);
}