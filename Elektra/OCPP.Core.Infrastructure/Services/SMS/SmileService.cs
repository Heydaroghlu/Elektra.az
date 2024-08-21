using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Infrastructure.Services.SMS
{
    public class SmileService
    {
        public async Task<string> Send(string number, string message)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"https://html.smilesms.az:8443/SmileWS2/webSmpp.jsp?username=2521&password=AZksm30El&numberId=1634&msisdn=994{number}&msgBody={message}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    return (e.Message);

                }
            }
            return "ok";
        }
        public async Task<string> RandomCode()
        {
            Random random = new Random();
            int sixDigitNumber = random.Next(1000, 9999);
            return sixDigitNumber.ToString();
        }
    }
}
