using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Application.DTOs.LocationDTOs;
using OCPP.Core.Application.DTOs.TarifDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;

namespace OCPP.Core.Application.DTOs.StatusDTOs
{
    public class ConnectorStatusDTO
    {
        public int Status { get; set; }
        public DateTime StartTime { get; set; }
        public double? ChargeRateKW { get; set; }  // Nullable double
        public int? TransactionId { get; set; }
        public string UserId { get; set; }
        public UserReturnDTO User { get; set; }
        public string Message { get; set; }
        public double? MeterKWH { get; set; }      // Nullable double
        public double? SoC { get; set; }           // Nullable double
    }

    public class ChargePointStatus
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Protocol { get; set; }
        public LocationReturnDTO Location { get; set; }
        public TarinfReturnDTO Tarif { get; set; }
        public Dictionary<int, ConnectorStatusDTO>? OnlineConnectors { get; set; }
    }
}
