namespace OCPP.Core.Database;

public class PaymentStatus:BaseEntity
{
    public string MemberId { get; set; }
    public string OrderId { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
}