using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace OCPP.Core.Server.Messages_OCPP16
{
    public class RemoteStopTransactionRequest
    {
        [JsonProperty("transactionId")]
        public int TransactionId { get; set; }

        [JsonProperty("idTag")]
        public string IdTag { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }


    }
}
