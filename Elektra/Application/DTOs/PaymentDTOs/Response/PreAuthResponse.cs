using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OCPP.Core.Application.DTOs.PaymentDTOs.Response;

public class PreAuthResponse
{
    [JsonProperty("statusCode")]
    public string StatusCode { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("bankOrderID")]
    public string BankOrderId { get; set; }

    [JsonProperty("bankSessionID")]
    public string BankSessionId { get; set; }

    [JsonProperty("clientOrderId")]
    public string ClientOrderId { get; set; }

    [JsonProperty("transactionId")]
    public int TransactionId { get; set; }
}