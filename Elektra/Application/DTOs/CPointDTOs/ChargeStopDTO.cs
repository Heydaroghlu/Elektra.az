using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.CPointDTOs
{
    public class ChargeStopDTO
    {
        public int TransactionId { get; set; }

        public string IdTag { get; set; }

        public string Reason { get; set; }
    }
}
