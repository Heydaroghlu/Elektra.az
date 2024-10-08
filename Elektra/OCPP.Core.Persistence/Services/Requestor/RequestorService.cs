using System.Text.Json;
using Application.Enums;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.UnitOfWorks;
using OCPP.Core.Persistence.Services.Payment;
using OCPP.Core.Persistence.Services.Payment;
using OCPP.Core.Persistence.UnitOfWorks;

namespace OCPP.Core.Persistence.Services.Requestor;

public class RequestorService
{
    IUnitOfWork _unitOfWork;
    PaymentService _paymentService;

    public RequestorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _paymentService = new PaymentService(_unitOfWork);
    }
    public async Task<string> Login()
    {
        PaymentLoginDTO login = new PaymentLoginDTO()
        {
            Email = "info@alcha.net",
            Password = "Et2y2PU0N!Zr"
        };
        var result = _paymentService.Auth(login);
        return (result.Result);
    }
    public async Task<string> Requestor<TEntity>(TEntity entity,string UrlKey,MethodType type )
    {
        var data = await _unitOfWork.RepositoryUrl.GetAsync((x => x.Key == UrlKey));
        if (data == null)
        {
            return ("Url is null");
        }
        string jsonString = JsonSerializer.Serialize(entity);
        string url = data.Value;
        string token=await Login();
        var result =await _paymentService.RequestAsync(url, jsonString, type.ToString(),token);
        return result;
    }

    public bool Check(string response)
    {
        if (response.Contains("Hata710"))
        {
            return false;
        }

        return true;
    }
    public async Task<string> Preauth(Preauth preauth)
    {
        var result = await Requestor(preauth, "Preauth", MethodType.Post);
        if (!Check(result))
        {
            return null;
        }
        return result;
    }
        public async Task<string> Checkout(CheckoutDTO checkoutDto)
        {
            var result = await Requestor(checkoutDto,"Checkout",MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> CardRegistration(CardRegistration cardRegistration)
        {
            var result = await Requestor(cardRegistration,"CardRegistration",MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> GetCustomerCards(GetCustomerCards getCustomerCards)
        {
            var result = await Requestor(getCustomerCards, "GetCustomerCards", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> DeleteCustomerSavedCards(DeleteCustomerSavedCards deleteCustomerSavedCards)
        {
            var result=await Requestor(deleteCustomerSavedCards, "DeleteCustomerSavedCards", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> TransactionStatusByOrderId(TransactionStatusByOrderId transactionStatusByOrderId)
        {
            var result = await Requestor(transactionStatusByOrderId, "TransactionStatusByOrderId", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> TransactionStatusByTransactionId(TransactionStatusByTransactionId transactionStatusByTransactionId)
        {
            var result = await Requestor(transactionStatusByTransactionId, "TransactionStatusByTransactionId", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> PurchaseWithSavedCard3DS(PurchaseWithSavedCard3DS purchaseWithSavedCard3Ds)
        {
            var result = await Requestor(purchaseWithSavedCard3Ds, "PurchaseWithSavedCard3DS", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> CardRegistrationRecurring(CardRegistrationRecurring cardRegistrationRecurring)
        {
            var result = await Requestor(cardRegistrationRecurring, "CardRegistrationRecurring", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }

        public async Task<string> PreauthCompletion(PreauthCompletion preauthCompletion)
        {
            var result = await Requestor(preauthCompletion, "PreauthCompletion", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> Refund(Refund refund)
        {
            var result = await Requestor(refund, "Refund", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        public async Task<string> Reversal(Reversal reversal)
        {
            var result = await Requestor(reversal, "Reversal", MethodType.Post);
            if (!Check(result))
            {
                return null;
            }
            return result;
        }
        
   
}