using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class ConnectorDTO
    {
        public string ChargePointId { get; set; }
        public int ConnectorId { get; set; }
        public string LastStatus { get; set; }
        public int ReservMinute { get; set; }
        public string Type { get; set; }
        public string ReservUser {  get; set; }
        public DateTime? ReservTime { get; set; }
    }
}
