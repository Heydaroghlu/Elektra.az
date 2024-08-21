using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public sealed partial class GetCustomerCards
    {
        [JsonProperty("memberId")]
        public string MemberId { get; set; }

        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }
    }
}
