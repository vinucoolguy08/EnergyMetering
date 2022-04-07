using System.ComponentModel.DataAnnotations;

namespace EnergyMetering.Models
{
    public class FilterEnergy
    {
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}
