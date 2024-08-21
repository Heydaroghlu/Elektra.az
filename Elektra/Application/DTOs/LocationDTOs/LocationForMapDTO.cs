using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.LocationDTOs
{
    public class LocationForMapDTO
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public CpForStatusDTO ChargePoint { get; set; }
    }
}
