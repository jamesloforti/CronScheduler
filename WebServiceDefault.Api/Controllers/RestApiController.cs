using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using WebServiceDefault.Common.Models;
using WebServiceDefault.Library.Providers.Interfaces;

namespace WebServiceDefault.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RestApiController : ControllerBase
    {
        private readonly ILogger<RestApiController> _logger;
        private readonly IExampleProvider _provider;

        public RestApiController(ILogger<RestApiController> logger, IExampleProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [HttpPost("SendAsync")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerOperation(Summary = "...",
            Description = "Required: ...")]
        public async Task<IActionResult> SendAsync([FromBody] ExampleRequest request)
        {
            _logger.Log(LogLevel.Information, $"{nameof(SendAsync)}:", new Dictionary<string, object>
            {
                { nameof(request.Name), request.Name }
            });

            try
            {
                var result = await _provider.SendAsync(request);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                _logger.Log(LogLevel.Error, $"{nameof(SendAsync)} failed:", new Dictionary<string, object>
                {
                    { nameof(request.Name), request.Name },
                    { nameof(ex.Data), JsonConvert.SerializeObject(ex.Data) }
                });
                return BadRequest(ex.Data?.Count > 0 ? ex.Message + JsonConvert.SerializeObject(ex.Data) : ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"{nameof(SendAsync)} failed:", new Dictionary<string, object>
                {
                    { nameof(request.Name), request.Name },
                    { nameof(ex.Data), JsonConvert.SerializeObject(ex.Data) }
                });
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Data?.Count > 0 ? ex.Message + JsonConvert.SerializeObject(ex.Data) : ex.Message);
            }
        }
    }
}
