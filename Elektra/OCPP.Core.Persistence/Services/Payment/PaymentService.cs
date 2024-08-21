using Newtonsoft.Json;
using OCPP.Core.Application.DTOs.PaymentDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Application.Helper;
using OCPP.Core.Application.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCPP.Core.Infrastructure.Services.Payment
{
    public class PaymentService
    {
        private IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        public PaymentService(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
        }
        public async Task<string> Request(string Url, string json,string method)
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // HttpClient kullanarak POST isteği gönderme
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(Url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                   return ("Başarılı! Gelen cevap: " + responseBody);
                }
                else
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                   return ("Başarısız! Durum kodu: " + response.StatusCode);
                }
            }
        }
        public async Task<string> Auth(PaymentLoginDTO paymentLogin)
        {
            //var url =await _unitOfWork.RepositoryPaymentSetting.GetAsync(x => x.Key == "Auth");
            string url = "https://test-vpos.unitedpayment.az/api/auth/";
            string json = JsonConvert.SerializeObject(paymentLogin);
            var result =await Request(url, json, "POST");
            return result;
        }
        /*public async Task<HttpResponseMessage> Checkout(CheckoutDTO checkout,string authToken)
        {
            string url = "https://test-vpos.unitedpayment.az/api/transactions/checkout";
            string jsonContent = JsonSerializer.Serialize(checkout);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Add the x-auth-token to the headers
            _httpClient.DefaultRequestHeaders.Add("x-auth-token", authToken);

            // Send the POST request
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            return response;
        }*/

        
    }
}
