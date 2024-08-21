using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class RejectReservDTO
    {
        public string ChargePointId { get; set; }
        public string AppUserId { get; set; }
        public int ConnectorId { get; set; }
    }
}
