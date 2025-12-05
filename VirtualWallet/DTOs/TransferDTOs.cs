using System.Runtime.InteropServices.JavaScript;
using VirtualWallet.Enums;

namespace VirtualWallet.DTOs;

public class TransferDTO
{
    public TransferType Type { get; set; }
    public float Amount { get; set; }
}

public class TransferSummaryDTO
{
    public float Amount { get; set; }
    public DateTime Date { get; set; }
    public TransferType Type { get; set; }
}

public class TransfersPaginatedDTO
{
    public List<TransferSummaryDTO> Transfers { get; set; } = new();
    public float PageNumber { get; set; }
    public float TotalPages { get; set; }
}