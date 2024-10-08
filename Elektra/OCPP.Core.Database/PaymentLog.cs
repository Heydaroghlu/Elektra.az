using System;

namespace OCPP.Core.Database;

public class PaymentLog:BaseEntity
{
    public string AppUserId { get; set; }
    public string TransactionId { get; set; }
    public string Status { get; set; }
    public string TransactionType { get; set; }
    public string ApiType { get; set; }
    public double Amount { get; set; }
    public bool End { get; set; }
    public DateTime CreatedAt { get; set; }
    public AppUser AppUser { get; set; }
}