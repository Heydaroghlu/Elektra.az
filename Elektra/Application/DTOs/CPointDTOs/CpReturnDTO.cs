using OCPP.Core.Application.DTOs.CpImageDTOs;
using OCPP.Core.Application.DTOs.LocationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCPP.Core.Application.DTOs.TarifDTOs;
using OCPP.Core.Database;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class CpReturnDTO
    {
        public string ChargePointId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public TarinfReturnDTO Tarif { get; set; }
        public LocationReturnDTO Location { get; set; }
        public List<ConnectorDTO> ConnectorStatus { get; set; }
        public List<CpImageDTO> CpImages { get; set; } 
    }
}
