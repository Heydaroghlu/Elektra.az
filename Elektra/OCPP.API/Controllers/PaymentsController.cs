using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Infrastructure.Services.Payment;
using OCPP.Core.Persistence.UnitOfWorks;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
         IUnitOfWork _unitOfWork;
        PaymentService _paymentService;
        public PaymentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //_paymentService=new PaymentService(unitOfWork);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(PaymentLoginDTO loginDTO)
        {
            var result = _paymentService.Auth(loginDTO);
            return Ok(result.Result);
        }
       
    }
}
