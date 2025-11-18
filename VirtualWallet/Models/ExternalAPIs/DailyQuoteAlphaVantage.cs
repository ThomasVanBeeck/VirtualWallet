using System.Text.Json.Serialization;

namespace VirtualWallet.Models.ExternalAPIs;

public class DailyQuoteAlphaVantage
{
    [JsonPropertyName("2. high")]
    public string High { get; set; } = string.Empty;

    [JsonPropertyName("3. low")]
    public string Low { get; set; } = string.Empty;
}