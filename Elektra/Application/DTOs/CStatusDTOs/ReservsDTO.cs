using OCPP.Core.Application.DTOs.CPointDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CStatusDTOs
{
    public class ReservsDTO
    {
        public string ChargePointId { get; set; }
        public int ConnectorId { get; set; }
        public string LastStatus { get; set; }
        public int ReservMinute { get; set; }
        public string ReservUser { get; set; }
        public CpReturnDTO ChargePoint { get; set; }
        public DateTime? ReservStart { get; set; }
        public DateTime? ReservTime { get; set; }
    }
}
