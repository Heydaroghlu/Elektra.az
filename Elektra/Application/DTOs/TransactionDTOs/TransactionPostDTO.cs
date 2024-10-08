using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;
using OCPP.Core.Database;

namespace OCPP.Core.Application.DTOs.TransactionDTOs;

public class TransactionPostDTO
{
    public int TransactionId { get; set; }
    public string ChargePointId { get; set; }
    public int ConnectorId { get; set; }
    public ChargeTagReturnDTO StartTag { get; set; }
    public DateTime StartTime { get; set; }
    public double MeterStart { get; set; }
    public string StartResult { get; set; }
    public ChargeTagReturnDTO StopTag { get; set; }
    public DateTime? StopTime { get; set; }
    public double? MeterStop { get; set; }
    public string StopReason { get; set; }
    public double TotalAmount { get; set; }
    public bool IsPayment { get; set; }
    //Hubdan gelen mawinin faizi
    public string EndPercent { get; set; }
    //Hubdan gelen mesaj (neche Kw istifade olunub?)
    public string EndMessage { get; set; }
    public ChargepointPostDTO ChargePoint { get; set; }
}