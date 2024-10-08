using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCPP.Core.Application.DTOs.TarifDTOs;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class CpForStatusDTO
    {
        public string ChargePointId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }  
        public TarinfReturnDTO Tarif { get; set; }
    }
}
