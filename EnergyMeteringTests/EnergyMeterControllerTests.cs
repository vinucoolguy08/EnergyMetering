using EnergyMetering.Controllers;
using EnergyMetering.Models;
using EnergyMetering.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var okResult =  result.Should().BeOfType<OkObjectResult>().Subject;
            var historicalList = okResult.Value.Should().BeAssignableTo<List<HistoricalData>>().Subject;
            historicalList.Count().Should().Be(0);
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
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var historicalList = okResult.Value.Should().BeAssignableTo<List<HistoricalData>>().Subject;
            historicalList.Count().Should().Be(1);
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
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var updatedEnergyReportList = okResult.Value.Should().BeAssignableTo<List<UpdatedEnergyReport>>().Subject;
            updatedEnergyReportList.Count().Should().Be(0);
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
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var updatedEnergyReportList = okResult.Value.Should().BeAssignableTo<List<UpdatedEnergyReport>>().Subject;
            updatedEnergyReportList.Count().Should().Be(1);
        }
    }
}