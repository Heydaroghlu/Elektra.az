using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class Wishlist:BaseEntity
    {
        public string AppUserId { get; set; }   
        public string ChargePointId { get; set; }
        public AppUser AppUser { get; set; }
        public ChargePoint ChargePoint  { get; set; }
    }
}
