using OCPP.Core.Application.DTOs.CPointDTOs;
using OCPP.Core.Application.DTOs.UserDTOs;

namespace OCPP.Core.Application.DTOs.ReservDTOs;

public class ReservPostDTO
{
    public int Id { get; set; }
    public string AppUserId { get; set; }
    public string ChargePointId {  get; set; }
    public int ConnectorId {  get; set; }
    public string LastStatus { get; set; }
    public string NewStatus { get; set; }
    public DateTime StartReserv { get; set; }
    public DateTime ReservTime {  get; set; }
    public CpReturnDTO ChargePoint { get; set; }
    public UserReturnDTO AppUser { get; set; }
    
}