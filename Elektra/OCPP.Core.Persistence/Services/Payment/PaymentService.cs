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
using System.Text.Json;
using System.Threading.Tasks;
using Application.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCPP.Core.Persistence.Services.Payment
{
    public class PaymentService
    {
        private IUnitOfWork _unitOfWork;
        private readonly HttpClient _client;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _client = new HttpClient();
        }
        public async Task<string> RequestAsync(string url, string json,string method,string? token=null)
        {
            try
            {
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            
                HttpResponseMessage response = null;
            
                switch (method.ToUpper())
                {
                    case "POST":
                        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        response = await _client.PostAsync(url, content);
                        break;
                    case "PUT":
                        response = await _client.PutAsync(url, content);
                        break;
                    default:
                        throw new ArgumentException("Unsupported HTTP method");
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    return "Hata710" + response;
                }
            }
            catch (Exception ex)
            {
                return "Hata710: " + ex.Message;
            }
        }
        public async Task<string> Auth(PaymentLoginDTO paymentLogin)
        {
            string url = "https://vpos.unitedpayment.az/api/auth/";
            string json = JsonConvert.SerializeObject(paymentLogin);
            var result =await RequestAsync(url, json, "POST");
            var jsonDoc = JsonDocument.Parse(result);
            if (jsonDoc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
            {
                return tokenElement.GetString(); // Token'ı string olarak döndür
            }
            else
            {
                return "Token is notfound";
            }
            return result;
        }

        
    }
}
