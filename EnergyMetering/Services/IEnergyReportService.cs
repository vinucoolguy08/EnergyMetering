using EnergyMetering.Models;

namespace EnergyMetering.Services
{
    public interface IEnergyReportService
    {
        Task<List<Customer>> GetEnergyReport();
        Task<List<HistoricalData>> FindMeterReadings(Guid MeterId, FilterEnergy filterEnergy);
        Task<List<UpdatedEnergyReport>> RequestEnergyReports(List<EnergyReportRequest> energyReports);
    }
}
