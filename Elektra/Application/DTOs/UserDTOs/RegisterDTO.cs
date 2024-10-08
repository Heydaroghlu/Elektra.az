using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.UserDTOs
{
    public class RegisterDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
