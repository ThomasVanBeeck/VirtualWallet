namespace VirtualWallet.Dtos;


public class WalletSummaryDto
{
    public required TransfersPaginatedDto TransferPage { get; set; }
    public List<HoldingSummaryDto> Holdings { get; set; } = new();
    public float TotalCash { get; set; }
    public float TotalInStocks { get; set; }
    public float TotalProfit  { get; set; }
    public float WinLossPct { get; set; }
}