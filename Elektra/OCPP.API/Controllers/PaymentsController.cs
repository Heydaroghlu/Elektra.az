using System.Text.Json;
using Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
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
            _paymentService=new PaymentService(_unitOfWork);
        }
       
        private async Task<string> Login()
        {
            PaymentLoginDTO login = new PaymentLoginDTO()
            {
                Email = "support@unitedpayment.com",
                Password = "Testmerchant12!"
            };
            var result = _paymentService.Auth(login);
            return (result.Result);
        }

        private async Task<string> Requestor<TEntity>(TEntity entity,string UrlKey,MethodType type )
        {
            var data = await _unitOfWork.RepositoryUrl.GetAsync((x => x.Key == UrlKey));
            if (data.Value == null)
            {
                return ("Url is null");
            }
            string jsonString = JsonSerializer.Serialize(entity);
            string url = data.Value;
            string token=await Login();
            var result =await _paymentService.RequestAsync(url, jsonString, type.GetDisplayName(),token);
            return result;
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(CheckoutDTO checkoutDto)
        {
            var result = await Requestor(checkoutDto,"Checkout",MethodType.Post);
            return Ok(result);
        }
        [HttpPost("CardRegistration")]
        public async Task<IActionResult> CardRegistration(CardRegistration cardRegistration)
        {
            var result = await Requestor(cardRegistration,"CardRegistration",MethodType.Post);
            return Ok(result);
        }

        [HttpGet("GetCustomerCards")]
        public async Task<IActionResult> GetCustomerCards(GetCustomerCards getCustomerCards)
        {
            var result = await Requestor(getCustomerCards, "GetCustomerCards", MethodType.Get);
            return Ok(result);
        }

        [HttpPost("DeleteCustomerSavedCards")]
        public async Task<IActionResult> DeleteCustomerSavedCards(DeleteCustomerSavedCards deleteCustomerSavedCards)
        {
            var result=await Requestor(deleteCustomerSavedCards, "DeleteCustomerSavedCards", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("TransactionStatusByOrderId")]
        public async Task<IActionResult> TransactionStatusByOrderId(TransactionStatusByOrderId transactionStatusByOrderId)
        {
            var result = await Requestor(transactionStatusByOrderId, "TransactionStatusByOrderId", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("TransactionStatusByTransactionId")]
        public async Task<IActionResult> TransactionStatusByTransactionId(TransactionStatusByTransactionId transactionStatusByTransactionId)
        {
            var result = await Requestor(transactionStatusByTransactionId, "TransactionStatusByTransactionId", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("PurchaseWithSavedCard3DS")]
        public async Task<IActionResult> PurchaseWithSavedCard3DS(PurchaseWithSavedCard3DS purchaseWithSavedCard3Ds)
        {
            var result = await Requestor(purchaseWithSavedCard3Ds, "PurchaseWithSavedCard3DS", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("CardRegistrationRecurring")]
        public async Task<IActionResult> CardRegistrationRecurring(CardRegistrationRecurring cardRegistrationRecurring)
        {
            var result = await Requestor(cardRegistrationRecurring, "CardRegistrationRecurring", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("Preauth")]
        public async Task<IActionResult> Preauth(Preauth preauth)
        {
            var result = await Requestor(preauth, "Preauth", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("PreauthCompletion")]
        public async Task<IActionResult> PreauthCompletion(PreauthCompletion preauthCompletion)
        {
            var result = await Requestor(preauthCompletion, "PreauthCompletion", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("Refund")]
        public async Task<IActionResult> Refund(Refund refund)
        {
            var result = await Requestor(refund, "Refund", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("Reversal")]
        public async Task<IActionResult> Reversal(Reversal reversal)
        {
            var result = await Requestor(reversal, "Reversal", MethodType.Post);
            return Ok(result);
        }
       
    }
}
