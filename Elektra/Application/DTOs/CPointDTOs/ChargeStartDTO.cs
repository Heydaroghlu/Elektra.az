using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class ChargeStartDTO
    {
        public string UserId { get; set; }
        public string ChargePointId {  get; set; }
        public int ConnectorId { get; set; } 
        public int? EndKw {  get; set; } 
        public double? EndCash { get; set; }
        public int? EndTime { get; set; }   

    }
}
