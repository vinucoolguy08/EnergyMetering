namespace EnergyMetering.Models
{
    public class HistoricalData
    {
        public Guid Meter_Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double KiloWatt { get; set; }
    }
}
