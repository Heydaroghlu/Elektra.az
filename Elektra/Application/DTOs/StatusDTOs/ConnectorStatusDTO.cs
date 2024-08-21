using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.StatusDTOs
{
    public class ConnectorStatusDTO
    {
        public int Status { get; set; }
        public double? ChargeRateKW { get; set; }  // Nullable double
        public double? MeterKWH { get; set; }      // Nullable double
        public double? SoC { get; set; }           // Nullable double
    }

    public class ChargePointStatus
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Protocol { get; set; }
        public Dictionary<int, ConnectorStatusDTO>? OnlineConnectors { get; set; }
    }
}
