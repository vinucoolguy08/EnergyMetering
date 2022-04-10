using EnergyMetering.Exceptions;
using EnergyMetering.Models;
using EnergyMetering.Repository;
using Microsoft.EntityFrameworkCore;

namespace EnergyMetering.Services
{
    public class EnergyReportService : IEnergyReportService
    {
        private readonly DataContext _context;

        public EnergyReportService(DataContext apiContext)
        {
            _context = apiContext;
            _context.Database.EnsureCreated();
        }

        public async Task<List<Customer>> GetEnergyReport()
        {
            return await _context.Customer.Include(x => x.MeterReadings).ToListAsync();
        }

        public async Task<List<HistoricalData>> FindMeterReadings(Guid MeterId, FilterEnergy filterEnergy)
        {
            var meterReadingsList = await _context.Customer.Include(x => x.MeterReadings).Where(x => x.MeterId.Equals(MeterId)).SelectMany(x => x.MeterReadings).ToListAsync();

            var result = meterReadingsList.Where(x => x.TimeStamp >= filterEnergy.FromDate && x.TimeStamp <= filterEnergy.ToDate).Select(x => new HistoricalData { Meter_Id = MeterId, KiloWatt = x.Kilowatt, Timestamp = x.TimeStamp }).ToList();

            if (!result.Any())
            {
                throw new NotFoundException($"No Meter Reading details info found");
            }
            return result;
        }

        public async Task<List<UpdatedEnergyReport>> RequestEnergyReports(List<EnergyReportRequest> energyReports)
        {
            List<UpdatedEnergyReport> updatedEnergyReportsList = new List<UpdatedEnergyReport>();

            foreach (var report in energyReports)
            {
                var customer = await _context.Customer.Where(x => x.MeterId == report.MeterId && x.DeliveryEmail == report.DeliveryEmail).FirstOrDefaultAsync();
                if (customer != null)
                {
                    customer.Status = "Pending";
                    updatedEnergyReportsList.Add(new UpdatedEnergyReport { ResponseId = customer.MeterId, Status = customer.Status });
                }
                else
                {
                    throw new NotFoundException($"The {report.MeterId} associated withe {report.DeliveryEmail} does not exist");
                }
            }
            if (_context.ChangeTracker.HasChanges())
            {
                _context.SaveChanges();
            }
            return updatedEnergyReportsList;
        }
    }
}
