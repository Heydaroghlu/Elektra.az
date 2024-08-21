using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class BaseEntity
    {
        public int Id { get; set; } 
        public bool Deleted { get; set; }
    }
}
