namespace OCPP.Core.Application.DTOs.DashboardDTOs;

public class DashboardDTO
{
    public int Station { get; set; }
    public int Active { get; set; }
    public int DeActive { get; set; }
    public int Seans { get; set; }
    public int ActiveSeans { get; set; }
    public int UsedEnergy { get; set; }
    public double Cost { get; set; }

}