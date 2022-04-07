namespace EnergyMetering.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string DeliveryEmail { get; set; }
        public string Status { get; set; }
        public Guid MeterId { get; set; }
        public virtual ICollection<MeterReading> MeterReadings { get; set; }
    }
}
