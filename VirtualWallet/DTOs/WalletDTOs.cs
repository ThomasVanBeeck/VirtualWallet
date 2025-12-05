namespace VirtualWallet.DTOs;

/*
public class WalletDTO
{
    public List<TransferDTO> Transfers { get; set; }
    public List<HoldingDTO> Holdings { get; set; }
}
*/


public class WalletSummaryDTO
{
    public TransfersPaginatedDTO TransferPage { get; set; }
    public List<HoldingSummaryDTO> Holdings   { get; set; }
    public float TotalCash { get; set; }
    public float TotalInStocks { get; set; }
    public float TotalProfit  { get; set; }
    public float WinLossPct { get; set; }
}