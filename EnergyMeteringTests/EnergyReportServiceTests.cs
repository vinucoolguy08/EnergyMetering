using EnergyMetering.Models;
using EnergyMetering.Repository;
using EnergyMetering.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EnergyMeteringTests
{
    public class EnergyReportServiceTests : IDisposable
    {
        private readonly DataContext _dbContext;

        public EnergyReportServiceTests()
        {
            _dbContext = new DataContext(CreateNewContextOptions());
        }

        [Fact]
        public async Task Should_ReturnEmpty_WhenMeterIdDoesNotExistInDb()
        {
            // Arrange
            Guid meterId = await GenerateInMemoryDB();
            var customer = await _dbContext.Customer.FirstOrDefaultAsync();
            FilterEnergy filterEnergy = new FilterEnergy();
            EnergyReportService reportService = new EnergyReportService(_dbContext);

            // Act
            var result = await reportService.FindMeterReadings(Guid.NewGuid(), new FilterEnergy());

            // Assert
            result.Should().HaveCount(0);
            result.Should().NotContain(x => x.Meter_Id == customer.MeterId);
        }

        [Fact]
        public async Task Should_ReturnHistoricalRecord_WhenMeterIdExistInDb()
        {
            // Arrange
            Guid meterId = await GenerateInMemoryDB();
            var customer =  await _dbContext.Customer.FirstOrDefaultAsync();
            FilterEnergy filterEnergy = new FilterEnergy();
            EnergyReportService reportService = new EnergyReportService(_dbContext);

            // Act
            var result = await reportService.FindMeterReadings(meterId, new FilterEnergy());    
            
            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Meter_Id == customer.MeterId);
        }

        [Fact]
        public async Task Should_NotReturnUpdatedEnergyRecord_WhenTheRequestedEnergyReportDoesNotExistInDb()
        {
            // Arrange
            Guid meterId = await GenerateInMemoryDB();
            var customer = await _dbContext.Customer.FirstOrDefaultAsync();
            List<EnergyReportRequest> energyReportList = new List<EnergyReportRequest>()
            {
                new EnergyReportRequest { MeterId = Guid.NewGuid() , DeliveryEmail = customer.DeliveryEmail }
            };
            EnergyReportService reportService = new EnergyReportService(_dbContext);

            // Act
            var result = await reportService.RequestEnergyReports(energyReportList);

            // Assert
            result.Should().HaveCount(0);
            result.Should().NotContain(x => x.ResponseId == customer.MeterId && x.Status == "Pending");
        }

        [Fact]
        public async Task Should_ReturnUpdatedEnergyRecord_WhenTheRequestedEnergyReportExistInDb()
        {
            // Arrange
            Guid meterId = await GenerateInMemoryDB();
            var customer = await _dbContext.Customer.FirstOrDefaultAsync();
            List<EnergyReportRequest> energyReportList = new List<EnergyReportRequest>()
            {
                new EnergyReportRequest { MeterId = meterId, DeliveryEmail = customer.DeliveryEmail }
            };
            EnergyReportService reportService = new EnergyReportService(_dbContext);

            // Act
            var result = await reportService.RequestEnergyReports(energyReportList);

            // Assert
            result.Should().HaveCount(1);
            result.Should().Contain(x => x.ResponseId == customer.MeterId && x.Status == "Pending");
        }

        private async Task<Guid> GenerateInMemoryDB()
        {
            var meterId = Guid.NewGuid();

            await _dbContext.Customer.AddRangeAsync(
                new Customer
                {
                    CustomerId = 1,
                    DeliveryEmail = "Sam@hotmail.com",
                    Status = "Approved",
                    MeterId = meterId,
                },
                new Customer
                {
                    CustomerId = 2,
                    DeliveryEmail = "Patrick@hotmail.com",
                    Status = "Approved",
                    MeterId = meterId,
                }
            );

            await _dbContext.MeterReading.AddRangeAsync(
                 new MeterReading
                 {
                     Id = 1,
                     Kilowatt = 12.5,
                     TimeStamp = DateTime.Now.AddDays(-1),
                     CustomerId = 1
                 },
                new MeterReading
                {
                    Id = 2,
                    Kilowatt = 22.5,
                    TimeStamp = DateTime.Now.AddDays(2),
                    CustomerId = 1,
                },
                new MeterReading
                {
                    Id = 3,
                    Kilowatt = 22.5,
                    TimeStamp = DateTime.Now.AddDays(-1),
                    CustomerId = 2,
                },
                new MeterReading
                {
                    Id = 4,
                    Kilowatt = 22.5,
                    TimeStamp = DateTime.Now.AddDays(2),
                    CustomerId = 2,
                });

            await _dbContext.SaveChangesAsync();
            return meterId;
        }

        private DbContextOptions<DataContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

    }
}