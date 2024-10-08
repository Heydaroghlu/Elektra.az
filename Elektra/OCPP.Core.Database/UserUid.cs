namespace OCPP.Core.Database;

public class UserUid:BaseEntity
{
    public string AppUserId { get; set; }
    public string CardUid { get; set; }
    public bool Selected { get; set; }
    public AppUser AppUser { get; set; }
}