using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.WishListDTOs
{
    public class WishListDTO
    {
        public int Id { get; set; } 
        public string AppUserId { get; set; }
        public CReturnOtherDTO ChargePoint { get; set; }
    }
}
