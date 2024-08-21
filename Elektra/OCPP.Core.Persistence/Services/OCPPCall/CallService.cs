using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OCPP.Core.Persistence.Services.OCPPCall
{
    public class CallService<TEntity> where TEntity : class
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;
        
        public CallService()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler);
        }
        public async Task<HttpResponseMessage> Send<TEntity>(string url, TEntity data)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, content);
        }
        public async Task<HttpResponseMessage> Get(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            return response;
        }
    }
}
