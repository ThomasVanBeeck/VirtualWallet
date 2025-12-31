using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class ScheduleTimerRepository: AbstractBaseRepository<ScheduleTimer>, IScheduleTimerRepository
{
    public ScheduleTimerRepository(AppDbContext context): base(context)
    { }

    public async Task<ScheduleTimer?> GetByTimestampKeyAsync(string timestampKey)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == timestampKey);
    }
    
    public async Task UpdateOrAddAsync(ScheduleTimer timer)
    {
        var existingTimer = await DbSet.FindAsync(timer.Key);
        if (existingTimer == null)
        {
            DbSet.Add(timer);
        }
        else
        {
            existingTimer.Value = timer.Value;
        }
    }
}