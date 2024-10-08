namespace OCPP.Core.Database;

public class VersionHistory
{
    public int Id { get; set; }
    public string Version { get; set; }
    public bool IsCritic { get; set; }
    public bool IsDeleted { get; set; }
}