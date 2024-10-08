
namespace OCPP.Core.Infrastructure.Services.PushNotification;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PushNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly List<NotificationResponse> _notifications; // Bildirimleri saklamak için liste

    public PushNotificationService()
    {
        _httpClient = new HttpClient();
        _notifications = new List<NotificationResponse>(); // Bildirim listesi oluştur
    }

    public async Task SendPushNotificationAsync(string expoPushToken, string title, string body)
    {
        var message = new
        {
            to = expoPushToken,
            sound = "default",
            title,
            body,
            data = new { someData = "goes here" }
        };

        var jsonMessage = JsonConvert.SerializeObject(message);
        var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://exp.host/--/api/v2/push/send", httpContent);
        response.EnsureSuccessStatusCode();

        // Bildirimi listeye ekle
        _notifications.Add(new NotificationResponse { Title = title, Body = body });
    }

    // Bildirimleri alma metodu
    public IEnumerable<NotificationResponse> GetNotifications()
    {
        return _notifications; // Saklanan bildirimleri döndür
    }
}

public class NotificationResponse
{
    public string Title { get; set; }
    public string Body { get; set; }
}
