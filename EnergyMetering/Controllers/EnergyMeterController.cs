using EnergyMetering.Models;
using EnergyMetering.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnergyMetering.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class EnergyMeterController : Controller
    {
        private readonly IEnergyReportService _energyReportService;

        public EnergyMeterController(IEnergyReportService energyReportService)
        {
            _energyReportService = energyReportService;
        }

        /// <summary>
        /// Gets all the customer related info [This API is only for reference]
        /// </summary>
        /// <returns>All the customer related info</returns>
        /// <response code="200">Returns the customer details</response>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _energyReportService.GetEnergyReport();
            return Ok(result);
        }


        /// <summary>
        /// Gets a specific Meter Reading Information
        /// </summary>
        /// <returns>Meter Reading info for a given time period</returns>
        /// <response code="200">Returns the Meter Reading details</response>
        /// <response code="404">No Meter Reading details info found</response>
        [HttpGet("MeterReadings/{MeterId}")]
        public async Task<IActionResult> GetMeterReadings(Guid MeterId, [FromQuery] FilterEnergy filterEnergy)
        {
            var result = await _energyReportService.FindMeterReadings(MeterId, filterEnergy);
            return Ok(result);
        }

        /// <summary>
        /// Request for Energy Report
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     [{
        ///        "DeliveryEmail": "vineshcool1990@gmail.com",
        ///        "MeterId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     }]
        ///
        /// </remarks>
        /// <returns>Requested report status</returns>
        /// <response code="200">Returns the status and responseid of the requested report</response>
        /// <response code="404">No report found for the given details</response>
        [HttpPost("RequestEnergyReports")]
        public async Task<IActionResult> Post([FromBody] List<EnergyReportRequest> energyReport)
        {
            var result = await _energyReportService.RequestEnergyReports(energyReport);
            return Ok(result);
        }
    }
}
