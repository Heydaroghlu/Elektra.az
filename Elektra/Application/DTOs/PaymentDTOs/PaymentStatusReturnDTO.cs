namespace OCPP.Core.Application.DTOs.PaymentDTOs;

public class PaymentStatusReturnDTO
{
    public int Id { get; set; }
    public string MemberId { get; set; }
    public string OrderId { get; set; }
    public double Amount { get; set; }
    public string Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
}