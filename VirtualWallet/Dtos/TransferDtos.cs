using System.Runtime.InteropServices.JavaScript;
using VirtualWallet.Enums;

namespace VirtualWallet.Dtos;

public class TransferDto
{
    public TransferType Type { get; set; }
    public float Amount { get; set; }
}

public class TransferSummaryDto
{
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public TransferType Type { get; set; }
}

public class TransfersPaginatedDto
{
    public List<TransferSummaryDto> Transfers { get; set; } = new();
    public float PageNumber { get; set; }
    public float TotalPages { get; set; }
}