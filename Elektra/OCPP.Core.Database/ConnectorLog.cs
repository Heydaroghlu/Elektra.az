using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class ConnectorLog:BaseEntity
    {
        public string AppUserId { get; set; }
        public string ChargePointId {  get; set; }
        public int ConnectorId {  get; set; }
        public string LastStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime StartReserv { get; set; }
        public DateTime ReservTime {  get; set; }   
        public AppUser AppUser { get; set; }
        public ChargePoint ChargePoint { get; set; }

    }
}
