namespace OCPP.Core.Application.DTOs.CPointDTOs;

public class MessageDTO
{
    public string ChargePointId { get; set; }
    public string ChargePointName { get; set; }
    public string Status { get; set; }
    public List<OnlineConnector> OnlineConnectors { get; set; }
}

public class OnlineConnector
{
    public int ConnectorId { get; set; }
    public DateTime StartTime { get; set; }
    public double Meter { get; set; }
    public double Charge { get; set; }
    public double SoC { get; set; }
}