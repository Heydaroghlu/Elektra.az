using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class CpImage:BaseEntity
    {
        public string Url { get; set; } 
        public bool IsPoster { get; set; }
        public string ChargePointId {  get; set; }    
        public ChargePoint ChargePoint { get; set; }
    }
}
