using OCPP.Core.Application.DTOs;
using OCPP.Core.Application.DTOs.CpImageDTOs;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class ChargepointPostDTO
    {
        public string ChargePointId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientCertThumb { get; set; }
        public int TarifId { get; set; }
        //public List<int> ConnectorId { get; set; }
        public List<CpImageDTO> CpImages { get; set; }
    }
}
