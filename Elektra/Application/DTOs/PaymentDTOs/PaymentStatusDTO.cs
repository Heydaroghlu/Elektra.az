namespace OCPP.Core.Application.DTOs.PaymentDTOs;

public class PaymentStatusDTO
{
    public string MemberId { get; set; }
    public string OrderId { get; set; }
    public double Amount { get; set; }
    public string? Description { get; set; }
}