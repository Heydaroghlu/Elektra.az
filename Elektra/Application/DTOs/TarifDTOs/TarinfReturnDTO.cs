using System.ComponentModel.DataAnnotations.Schema;

namespace OCPP.Core.Application.DTOs.TarifDTOs;

public class TarinfReturnDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int PowerKw { get;set; }
    [Column(TypeName = "decimal(18, 2)")]
    public double PriceForKw { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public double PriceForHour { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public double PriceForStartSession { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public double PriceForStartSeans { get; set; }
    public bool Reserv { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public double? PriceForReserv { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public double? PriceForReserv2 { get; set; }
}