namespace EnergyMetering.Exceptions
{
    public abstract class EnergyReportException : Exception
    {
        public string Name { get; set; }

        public EnergyReportException(string message) : base(message)
        {

        }
    }

    public class NotFoundException : EnergyReportException
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
