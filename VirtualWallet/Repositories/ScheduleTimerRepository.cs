using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class ScheduleTimerRepository
{
    private readonly AppDbContext _context;

    public ScheduleTimerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleTimer?> GetAsync(string timestampKey)
    {
        return await _context.ScheduleTimers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == timestampKey);
    }
    
    public async Task UpdateOrAddAsync(ScheduleTimer timer)
    {
        var existingTimer = await _context.ScheduleTimers.FindAsync(timer.Key);
        if (existingTimer == null)
        {
            _context.ScheduleTimers.Add(timer);
        }
        else
        {
            existingTimer.Value = timer.Value;
        }
        await _context.SaveChangesAsync();
    }
}