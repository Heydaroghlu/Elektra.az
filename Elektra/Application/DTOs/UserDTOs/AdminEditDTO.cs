namespace OCPP.Core.Application.DTOs.UserDTOs;

public class AdminEditDTO
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? NewPassword { get; set; }
}