﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public sealed partial class Preauth
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("memberId")]
        public string MemberId { get; set; }

        [JsonProperty("successUrl")]
        public string SuccessUrl { get; set; }

        [JsonProperty("cancelUrl")]
        public string CancelUrl { get; set; }

        [JsonProperty("declineUrl")]
        public string DeclineUrl { get; set; }
    }
}
