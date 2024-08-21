using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public sealed partial class DeleteCustomerSavedCards
    {
        [JsonProperty("memberId")]
        public string MemberId { get; set; }

        [JsonProperty("cardUID")]
        public string CardUID { get; set; }

        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }
    }
}
