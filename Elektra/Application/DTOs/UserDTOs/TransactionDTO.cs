using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.UserDTOs
{
    public class TransactionDTO
    {
        public string TransactionId { get; set; }
        public string ChargePointId { get; set; }
        public int ConnectorId { get; set; }
        public CpReturnDTO ChargePoint { get; set; }
    }
}
