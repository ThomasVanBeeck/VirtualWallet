using System.Text.Json.Serialization;
namespace VirtualWallet.Models.ExternalAPIs;

public class StockAlphaVantage
{
    [JsonPropertyName("Time Series (Daily)")]
    public Dictionary<string, DailyQuoteAlphaVantage> TimeSeriesDaily { get; set; } = new Dictionary<string, DailyQuoteAlphaVantage>();
}