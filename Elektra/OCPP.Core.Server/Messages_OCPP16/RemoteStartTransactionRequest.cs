namespace OCPP.Core.Server.Messages_OCPP16
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.1.0 (Newtonsoft.Json v9.0.0.0)")]
    public partial class RemoteStartTransactionRequest
    {

        [JsonProperty("idTag", Required = Required.Always)]
        [Required(AllowEmptyStrings = true)]
        [StringLength(20)]
        public string IdTag { get; set; }

        [JsonProperty("connectorId", Required = Required.Always)]
        public int ConnectorId { get; set; }

        [JsonProperty("chargingProfile", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ChargingProfile ChargingProfile { get; set; }
    }

    public class ChargingProfile
    {
        [JsonProperty("chargingProfileId", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int? ChargingProfileId { get; set; }

        [JsonProperty("stackLevel", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int? StackLevel { get; set; }

        [JsonProperty("chargingProfilePurpose", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ChargingProfilePurpose { get; set; }

        [JsonProperty("chargingProfileKind", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ChargingProfileKind { get; set; }

        [JsonProperty("chargingSchedule", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ChargingSchedule ChargingSchedule { get; set; }
    }

    public class ChargingSchedule
    {
        [JsonProperty("chargingRateUnit", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ChargingRateUnit { get; set; }

        [JsonProperty("chargingSchedulePeriod", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ChargingSchedulePeriod[] ChargingSchedulePeriod { get; set; }

        [JsonProperty("minChargingRate", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int? MinChargingRate { get; set; }
    }

    public class ChargingSchedulePeriod
    {
        [JsonProperty("startPeriod", Required = Required.Always)]
        public int StartPeriod { get; set; }

        [JsonProperty("limit", Required = Required.Always)]
        public int Limit { get; set; }
    }

}
