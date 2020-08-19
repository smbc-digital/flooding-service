using System;
using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;

namespace flooding_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class HomeController : ControllerBase
    {
        private readonly IFloodingService _floodingService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IFloodingService floodingService, ILogger<HomeController> logger)
        {
            _floodingService = floodingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FloodingRequest model)
        {
            try
            {
                var result = await _floodingService.CreateCase(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"FloodingService:: Post:: Error message: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}