namespace OCPP.Core.Application.DTOs.PaymentDTOs;

public class CardInfo
{
    public string MemberId { get; set; }
    public string CardUID { get; set; }
    public string MaskedPan { get; set; }
    public string Brand { get; set; }
}