using System.Threading.Tasks;
using flooding_service.Controllers.Models;
using flooding_service.Services;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(IFloodingService floodingService)
        {
            _floodingService = floodingService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FloodingRequest model)
        {
            try
            {
                var result = await _floodingService.CreateCase(model);
                return Ok(result);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}