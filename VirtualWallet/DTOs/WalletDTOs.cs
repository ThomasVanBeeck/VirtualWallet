namespace VirtualWallet.DTOs;

public class WalletDTO
{
    public List<TransferDTO> Transfers { get; set; }
    public List<HoldingDTO> Holdings { get; set; }
}