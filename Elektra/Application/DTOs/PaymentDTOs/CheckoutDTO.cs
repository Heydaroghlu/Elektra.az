using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public class CheckoutDTO
    {
        [JsonPropertyName("clientOrderId")]
        public string ClientOrderId { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("successUrl")]
        public string SuccessUrl { get; set; }

        [JsonPropertyName("cancelUrl")]
        public string CancelUrl { get; set; }

        [JsonPropertyName("declineUrl")]
        public string DeclineUrl { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("memberId")]
        public string MemberId { get; set; }

        [JsonPropertyName("additionalInformation")]
        public string AdditionalInformation { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("clientName")]
        public string ClientName { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("partnerId")]
        public string PartnerId { get; set; }
    }
}
