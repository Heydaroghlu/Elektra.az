using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public class PaymentLoginDTO
    {
        public string Email { get; set; } = "support@unitedpayment.com";
        public string Password { get; set; } = "Testmerchant12";
    }
}
