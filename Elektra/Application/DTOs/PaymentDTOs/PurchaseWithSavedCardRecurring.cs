using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.DTOs.PaymentDTOs
{
    public class PurchaseWithSavedCardRecurring
    {
        public string ClientOrderId { get; set; }
        public string Amount { get; set; }
        public string MemberId { get; set; }
        public string CardUid { get; set; }
    }
}
