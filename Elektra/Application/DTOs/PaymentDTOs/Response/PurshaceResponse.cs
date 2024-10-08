namespace OCPP.Core.Application.DTOs.PaymentDTOs.Response;

public class PurshaceResponse
{
    public string StatusCode { get; set; }
    public string Status { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; }
    public string BankOrderID { get; set; }
    public string BankSessionID { get; set; }
    public string ClientOrderId { get; set; }
    public int TransactionId { get; set; }
}