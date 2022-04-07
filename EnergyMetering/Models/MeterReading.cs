namespace EnergyMetering.Models
{
    public class MeterReading
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Kilowatt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
