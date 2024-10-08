using Microsoft.AspNetCore.SignalR;

namespace OCPP.Core.Infrastructure.Hubs;

public class PointHub:Hub
{
    private readonly HttpClient _httpClient;
    public PointHub(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task SendStatus()
    {
            /*var response = await _httpClient.GetAsync("https://elektra.pp.az/api/ChargePoints/ActiveStatus");
            response.EnsureSuccessStatusCode(); 
            var data = await response.Content.ReadAsStringAsync();*/
            await Clients.All.SendAsync("ReceiveMessage", "data");
    }
}