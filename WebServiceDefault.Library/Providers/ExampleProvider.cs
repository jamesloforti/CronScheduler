using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebServiceDefault.Common.Models;
using WebServiceDefault.Library.Clients.Interfaces;
using WebServiceDefault.Library.Providers.Interfaces;

namespace WebServiceDefault.Library.Providers
{
    public class ExampleProvider : IExampleProvider
    {
        private readonly ILogger<ExampleProvider> _logger;
        private readonly IGenericHttpClient _genericHttpClient;

        public ExampleProvider(ILogger<ExampleProvider> logger, IGenericHttpClient genericHttpClient)
        {
            _logger = logger;
            _genericHttpClient = genericHttpClient;
        }

        public async Task<string> SendAsync(ExampleRequest request)
        {
            _logger.Log(LogLevel.Error, $"{nameof(ExampleProvider)} {nameof(SendAsync)}.");
            var result = await _genericHttpClient.GetAsync("https://httpstat.us/200");
            return result;
        }
    }
}
