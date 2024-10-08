using Newtonsoft.Json;

namespace OCPP.Core.Application.DTOs.PaymentDTOs.Response;

public class PreauthCompResponse
{
    [JsonProperty("clientOrderId")]
    public string ClientOrderId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("transactionType")]
    public string TransactionType { get; set; }

    [JsonProperty("transactionId")]
    public int TransactionId { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}