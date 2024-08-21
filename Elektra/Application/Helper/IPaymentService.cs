using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.Helper
{
    public interface IPaymentService
    {
        Task<bool> Auth();
        Task<bool> CheckOut();
    }
}
