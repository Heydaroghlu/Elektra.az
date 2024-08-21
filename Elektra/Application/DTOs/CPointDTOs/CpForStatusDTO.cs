using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class CpForStatusDTO
    {
        public string ChargePointId { get; set; }
        public string Status { get; set; }  
    }
}
