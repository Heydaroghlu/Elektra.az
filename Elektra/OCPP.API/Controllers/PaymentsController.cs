using System.Security.Claims;
using System.Text.Json;
using Application.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using OCPP.Core.Application.Abstractions;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.DTOs.PaymentDTOs.Response;
using OCPP.Core.Application.DTOs.TokenDTO;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Database;
using OCPP.Core.Infrastructure.Services.Token;
using OCPP.Core.Persistence.Services.Payment;
using OCPP.Core.Persistence.Services.Requestor;
using OCPP.Core.Persistence.UnitOfWorks;

namespace OCPP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        RequestorService _requestorService;
        private IMapper _mapper;
        private readonly ITokenHandler _tokenHandler;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtHelper _jwtHelper=new JwtHelper();
        public PaymentsController(IUnitOfWork unitOfWork,IMapper mapper,ITokenHandler tokenHandler,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _requestorService = new RequestorService(_unitOfWork);
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(CheckoutDTO checkoutDto)
        {
            var result = await _requestorService.Requestor(checkoutDto,"Checkout",MethodType.Post);
            
            return Ok(result);
        }
        [HttpPost("CardRegistration")]
        public async Task<IActionResult> CardRegistration(CardRegistration cardRegistration)
        {
            var result = await _requestorService.Requestor(cardRegistration,"CardRegistration",MethodType.Post);
            return Ok(result);
        }

        [HttpPost("GetCustomerCards")]
        public async Task<IActionResult> GetCustomerCards(GetCustomerCards getCustomerCards)
        {
            var cards = await _requestorService.Requestor(getCustomerCards, "GetCustomerCards", MethodType.Post);
            List<CardInfo> cardList = JsonConvert.DeserializeObject<List<CardInfo>>(cards);
            foreach (var card in cardList)
            {
                var exist = await _unitOfWork.RepositoryUserUid.GetAsync(x=>x.CardUid==card.CardUID);
                if (exist==null)
                {
                    UserUid uid = new UserUid()
                    {
                        AppUserId = getCustomerCards.MemberId,
                        CardUid = card.CardUID,
                        Selected = false
                    };
                    await _unitOfWork.RepositoryUserUid.InsertAsync(uid);
                }
            }

            await _unitOfWork.CommitAsync();
            return Ok(cards);
        }

        [HttpPost("PurchaseWithSavedCardRecurring")]
        public async Task<IActionResult> PurchaseWithSavedCardRecurring(
            PurchaseWithSavedCardRecurring purchaseWithSavedCardRecurring)
        {
            var result=await _requestorService.Requestor(purchaseWithSavedCardRecurring, "PurchaseWithSavedCardRecurring", MethodType.Post);
            AppUser user =await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == purchaseWithSavedCardRecurring.MemberId);
            if (!result.Contains("Hata710"))
            {
                PurshaceResponse purshaceResponse = JsonConvert.DeserializeObject<PurshaceResponse>(result);
                if (purshaceResponse.Status == "APPROVED")
                {
                    user.Balance += purshaceResponse.Amount;
                    await _unitOfWork.CommitAsync();
                }
                return Ok(purshaceResponse);
            }

            return BadRequest("Payment error");
        }
        [HttpPost("DeleteCustomerSavedCards")]
        public async Task<IActionResult> DeleteCustomerSavedCards(DeleteCustomerSavedCards deleteCustomerSavedCards)
        {
            var result=await _requestorService.Requestor(deleteCustomerSavedCards, "DeleteCustomerSavedCards", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("TransactionStatusByOrderId")]
        public async Task<IActionResult> TransactionStatusByOrderId(TransactionStatusByOrderId transactionStatusByOrderId)
        {
            var result = await _requestorService.Requestor(transactionStatusByOrderId, "TransactionStatusByOrderId", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("TransactionStatusByTransactionId")]
        public async Task<IActionResult> TransactionStatusByTransactionId(TransactionStatusByTransactionId transactionStatusByTransactionId)
        {
            var result = await _requestorService.Requestor(transactionStatusByTransactionId, "TransactionStatusByTransactionId", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("PurchaseWithSavedCard3DS")]
        public async Task<IActionResult> PurchaseWithSavedCard3DS(PurchaseWithSavedCard3DS purchaseWithSavedCard3Ds)
        {
            var result = await _requestorService.Requestor(purchaseWithSavedCard3Ds, "PurchaseWithSavedCard3DS", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("CardRegistrationRecurring")]
        public async Task<IActionResult> CardRegistrationRecurring(CardRegistrationRecurring cardRegistrationRecurring)
        {
            var result = await _requestorService.Requestor(cardRegistrationRecurring, "CardRegistrationRecurring", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("Preauth")]
        public async Task<IActionResult> Preauth(Preauth preauth)
        {
            var result = await _requestorService.Requestor(preauth, "Preauth", MethodType.Post);
            return Ok(result);
        }
        [HttpPost("PreauthSavedCards")]
        public async Task<IActionResult> Preauth(PreAuthSavedCard preauth)
        {
            var result = await _requestorService.Requestor(preauth, "PreauthWithSavedCardRecurring", MethodType.Post);
            PreauthCardResponse response=  JsonConvert.DeserializeObject<PreauthCardResponse>(result);

            return Ok(response);
        }

        [HttpPost("PreauthCompletion")]
        public async Task<IActionResult> PreauthCompletion(PreauthCompletion preauthCompletion)
        {
            var result = await _requestorService.Requestor(preauthCompletion, "PreauthCompletion", MethodType.Post);
            return Ok(result);
        }

        [HttpPost("Refund")]
        public async Task<IActionResult> Refund(Refund refund)
        {
            var result = await _requestorService.Requestor(refund, "Refund", MethodType.Post);
            return Ok(result);
        }
        [HttpPost("Reversal")]
        public async Task<IActionResult> Reversal(Reversal reversal)
        {
            var result = await _requestorService.Requestor(reversal, "Reversal", MethodType.Post);
            return Ok(result);
        }

        [HttpGet("Logs")]
        public async Task<IActionResult> Logs(string? AppuserId)
        {
            List<PaymentLog> data = new List<PaymentLog>();
            if (AppuserId != null)
            {
               data =  _unitOfWork.RepositoryPaymentLog.GetAllAsync(x => x.AppUserId == AppuserId).ToList();

            }
            else
            {
                data = _unitOfWork.RepositoryPaymentLog.GetAllAsync(x => x.AppUserId != null).ToList();
            }

            return Ok(data);
        }
        [HttpPost("PaymentLog")]
        public async Task<IActionResult> PaymentLog(PaymentLogDTO paymentLogDto)
        {
            AppUser user = await _unitOfWork.RepositoryUser.GetAsync(x => x.Id == paymentLogDto.AppUserId);
            if (user == null)
            {
                return NotFound();
            }
            PaymentLog paymentLog = _mapper.Map<PaymentLog>(paymentLogDto);
            paymentLog.CreatedAt = DateTime.UtcNow.AddHours(4);
            _unitOfWork.RepositoryPaymentLog.InsertAsync(paymentLog);
            if (paymentLogDto.ApiType == "Checkout")
            {
                if (paymentLogDto.TransactionType == "APPROVED")
                {
                    user.Balance += paymentLogDto.Amount;
                }
            }
            await _unitOfWork.CommitAsync();
            return Ok();
        }
        
       
    }
}
