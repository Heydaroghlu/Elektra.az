using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public sealed partial class Refund
    {
        public string Amount { get; set; }
        public int TransactionId { get; set; }
    }
}
