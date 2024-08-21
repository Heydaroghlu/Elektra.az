using OCPP.Core.Application.DTOs.LocationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class CReturnOtherDTO
    {
        public string ChargePointId { get; set; }
        public string Name { get; set; }
        public LocationReturnDTO Location { get; set; } 
    }
}
