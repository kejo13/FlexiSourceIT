using FlexiSourceIT.Models;
using FlexiSourceIT.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlexiSourceIT.Controllers
{
    public class RainfallController : ControllerBase
    {
        private readonly IRainfallService _rainfallService;
        private readonly ILogger<RainfallController> _logger;
        public RainfallController(IRainfallService rainfallService, ILogger<RainfallController> logger)
        {
            _rainfallService = rainfallService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<RainfallReadingResponse>> GetRainfallReadings(string stationId)
        {
            try
            {
                var readings = await _rainfallService.GetRainfallReadingsAsync(stationId);
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing your request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
