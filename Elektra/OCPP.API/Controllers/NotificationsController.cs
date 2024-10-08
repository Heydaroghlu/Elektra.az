using OCPP.Core.Infrastructure.Services.PushNotification;

namespace OCPP.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly PushNotificationService _pushNotificationService;

    public NotificationsController()
    {
        _pushNotificationService = new PushNotificationService();
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest("Token is required.");
        }
        await _pushNotificationService.SendPushNotificationAsync(request.Token, request.Title, request.Body);
        return Ok("Notification sent!");
    }
    [HttpGet]
    public ActionResult<IEnumerable<NotificationResponse>> GetNotifications()
    {
        var notifications = _pushNotificationService.GetNotifications();
        return Ok(notifications);
    }
}

public class NotificationRequest
{
    public string Token { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
