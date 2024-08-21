using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class UrlData:BaseEntity
    {
        public string Key {  get; set; }
        public string Value { get; set; }   
    }
}
