using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class ChargeLocation:BaseEntity
    {
        public string ChargePointId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public ChargePoint ChargePoint { get; set; }
    }
}
