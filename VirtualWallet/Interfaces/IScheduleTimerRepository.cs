using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IScheduleTimerRepository
{
    Task<ScheduleTimer?> GetAsync(string timestampKey);
    Task UpdateOrAddAsync(ScheduleTimer timer);
}