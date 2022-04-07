using EnergyMetering.Controllers;
using EnergyMetering.Models;
using EnergyMetering.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EnergyMeteringTests
{
    public class EnergyMeterControllerTests
    {
        [Fact]
        public async Task Should_ReturnNotFound_WhenMeterIdDoesntExist()
        {
            // Arrange
            var meterId = Guid.NewGuid();

           var historicalDataList = new List<HistoricalData>();
          
            var mock = new Mock<IEnergyReportService>();
            mock.Setup(m => m.FindMeterReadings(It.IsAny<Guid>(), It.IsAny<FilterEnergy>())).ReturnsAsync(historicalDataList);

            var controller = new EnergyMeterController(mock.Object);
            var filterEnergy = new FilterEnergy { FromDate = DateTime.Now, ToDate = DateTime.Now };

            // Act
            var result = await controller.GetMeterReadings(meterId, filterEnergy);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Should_ReturnMatchedRecord_WhenTheGivenRecordExistInDB()
        {
            // Arrange
            var meterId = Guid.NewGuid();

            List<HistoricalData> historicalDataList = new List<HistoricalData>()
            {
                new HistoricalData{ KiloWatt = 12.4, Meter_Id = meterId, Timestamp = DateTime.Now }
            };

            var mock = new Mock<IEnergyReportService>();
            mock.Setup(m => m.FindMeterReadings(It.IsAny<Guid>(), It.IsAny<FilterEnergy>())).ReturnsAsync(historicalDataList);

            var controller = new EnergyMeterController(mock.Object);
            var filterEnergy = new FilterEnergy { FromDate = DateTime.Now, ToDate = DateTime.Now };

            // Act
            var result = await controller.GetMeterReadings(meterId, filterEnergy);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Should_ReturnNotFound_WhenMeterIdAndDeliveryEmailDoesntExist()
        {
            // Arrange
            var meterId = Guid.NewGuid();

            var updatedEnergyReports = new List<UpdatedEnergyReport>();
            var energyReportRequests = new List<EnergyReportRequest>()
            {
               new EnergyReportRequest { DeliveryEmail = "Sasi@hotmail.com", MeterId = meterId }
            };

            var mock = new Mock<IEnergyReportService>();
            mock.Setup(m => m.RequestEnergyReports(It.IsAny<List<EnergyReportRequest>>())).ReturnsAsync(updatedEnergyReports);

            var controller = new EnergyMeterController(mock.Object);

            // Act
            var result = await controller.Post(energyReportRequests);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Should_ReturnUpdatedEnergyReport_WhenMeterIdAndDeliveryEmailExistInDB()
        {
            // Arrange
            var meterId = Guid.NewGuid();

            var updatedEnergyReports = new List<UpdatedEnergyReport>()
            {
                new UpdatedEnergyReport { ResponseId = meterId, Status = "Pending"}
            };

            var energyReportRequests = new List<EnergyReportRequest>()
            {
               new EnergyReportRequest { DeliveryEmail = "Sasi@hotmail.com", MeterId = meterId }
            };

            var mock = new Mock<IEnergyReportService>();
            mock.Setup(m => m.RequestEnergyReports(It.IsAny<List<EnergyReportRequest>>())).ReturnsAsync(updatedEnergyReports);

            var controller = new EnergyMeterController(mock.Object);

            // Act
            var result = await controller.Post(energyReportRequests);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}