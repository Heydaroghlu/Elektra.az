namespace OCPP.Core.Application.DTOs.PaymentDTOs;

public class PreAuthSavedCard
{
    public string ClientOrderId { get; set; }
    public string Amount { get; set; }
    public string MemberId { get; set; }
    public string CardUid { get; set; }
}