using System.ComponentModel.DataAnnotations;

namespace EnergyMetering.Models
{
    public class EnergyReport
    {
        [Key]
        public Guid? MeterId { get; set; }
        public DateTime? TimeStamp { get; set; } 
        public double? KiloWatt { get; set; }
        public string? DeliveryEmail { get; set; }
    }
}
