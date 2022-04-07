namespace EnergyMetering.Models
{
    public class EnergyReportRequest
    {
        public string DeliveryEmail { get; set; }
        public Guid MeterId { get; set; }
    }
}
