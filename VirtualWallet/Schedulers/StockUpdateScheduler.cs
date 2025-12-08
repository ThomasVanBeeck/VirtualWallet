using VirtualWallet.Services;

namespace VirtualWallet.Schedulers;

public class StockUpdateScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(20); 

    public StockUpdateScheduler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("StockUpdateScheduler started");
        await ExecuteScopedUpdate(stoppingToken);
        
        using var timer = new PeriodicTimer(_checkInterval);
        
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            await ExecuteScopedUpdate(stoppingToken);
        }
    }
    
    private async Task ExecuteScopedUpdate(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var stockService = scope.ServiceProvider.GetRequiredService<StockService>();
            
            await stockService.UpdateStockPrices();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in background service: {ex.Message}");
        }
    }
}