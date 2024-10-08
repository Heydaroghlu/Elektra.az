namespace OCPP.Core.Application.DTOs.PaymentDTOs;

public class PaymentLogReturnDTO
{
    public int Id { get; set; }
    public string AppUserId { get; set; }
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public string TransactionType { get; set; }
    public string ApiType { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedAt { get; set; }

}