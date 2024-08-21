using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public  class CardRegistrationRecurring
    {
        public string ClientOrderId { get; set; }
        public string Language { get; set; }
        public string MemberId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public string DeclineUrl { get; set; }
    }
}
