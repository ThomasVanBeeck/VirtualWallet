namespace VirtualWallet.Services;

public interface ISettingsService
{
    Task<DateTime> GetLastUpdateTimestampAsync();
    Task SetLastUpdateTimestampAsync(DateTime timestamp);
}