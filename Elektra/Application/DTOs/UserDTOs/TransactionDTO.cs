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
        public string StopReason { get; set; }
        //Hubdan gelen mawinin faizi
        public string EndPercent { get; set; }
        public double TotalAmount { get; set; }
        //odenish cixilibmi?
        public bool IsPayment { get; set; }
        //Hubdan gelen mesaj (neche Kw istifade olunub?)
        public string EndMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }

        public CpReturnDTO ChargePoint { get; set; }
    }
}
