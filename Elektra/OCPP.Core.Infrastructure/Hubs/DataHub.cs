using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace OCPP.Core.Infrastructure.Hubs;

public class DataHub : Hub
{
    private readonly HttpClient _httpClient;

    public DataHub(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task FetchAndSendData()
    {
        try
        {
            var response = await _httpClient.GetStringAsync("https://fakestoreapi.com/products");
            var data = JArray.Parse(response);
            await Clients.All.SendAsync("ReceiveData", data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"API çağrısı sırasında hata oluştu: {ex.Message}");
        }
    }
}
