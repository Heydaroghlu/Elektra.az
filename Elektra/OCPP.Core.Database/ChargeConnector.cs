using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class ChargeConnector:BaseEntity
    {
        public int ConnectorId { get; set; }
        public string ChargePointId {  get; set; }  
        public ChargePoint ChargePoint { get; set; }    
    }
}
