using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace WebServiceDefault.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ServiceStatus")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerOperation(Summary = "Returns the current status of the web service.")]
        public IActionResult ServiceStatus()
        {
            _logger.Log(LogLevel.Error, $"{nameof(ServiceStatus)}:");
            return Ok("Service is up and running.");
        }
    }
}
