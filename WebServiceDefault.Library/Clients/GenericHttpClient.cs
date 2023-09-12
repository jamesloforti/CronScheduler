using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using WebServiceDefault.Common.Settings;
using WebServiceDefault.Library.Clients.Interfaces;

namespace WebServiceDefault.Library.Clients
{
    public class GenericHttpClient : IGenericHttpClient
    {
        private readonly ILogger<GenericHttpClient> _logger;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;
        private HttpStatusCode[] _httpStatusCodesWorthRetrying;

        public GenericHttpClient(ILogger<GenericHttpClient> logger, AppSettings appSettings, HttpClient httpClient)
        {
            _logger = logger;
            _appSettings = appSettings;
            _httpClient = httpClient;
            _httpStatusCodesWorthRetrying = new HttpStatusCode[]
            {
                HttpStatusCode.RequestTimeout, // 408
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.ServiceUnavailable, // 503
                HttpStatusCode.GatewayTimeout // 504
            };
        }

        public async Task<T> GetAsync<T>(string url)
        {
            return JsonConvert.DeserializeObject<T>(await GetAsync(url));
        }

        public async Task<string> GetAsync(string url)
        {
            var resultStr = string.Empty;

            try
            {
                var response = await Policy
                    .Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => _httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                    .WaitAndRetryAsync(_appSettings.PollyMaxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (httpResponseMsg, attemptSeconds, context) =>
                    {
                        _logger.Log(LogLevel.Warning, httpResponseMsg?.Exception, $"{nameof(GetAsync)} failed, retrying:", new Dictionary<string, object>
                        {
                            { nameof(url), url},
                            { nameof(attemptSeconds), attemptSeconds },
                            { nameof(_appSettings.PollyMaxRetries), _appSettings.PollyMaxRetries },
                            { nameof(httpResponseMsg.Result.ReasonPhrase), httpResponseMsg?.Result?.ReasonPhrase },
                            { nameof(httpResponseMsg.Result.StatusCode), httpResponseMsg?.Result?.StatusCode }
                        });
                    })
                    .ExecuteAsync(() =>
                    {
                        var httpRequestMessage = new HttpRequestMessage
                        {
                            Method = HttpMethod.Get,
                            RequestUri = new Uri($"{url}")
                        };

                        return _httpClient.SendAsync(httpRequestMessage);
                    });

                resultStr = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                ex.Data.Add("Result", resultStr);
                _logger.Log(LogLevel.Error, ex, "Failed to complete http GET request:", new Dictionary<string, object>
                {
                    { nameof(url), url },
                    { nameof(resultStr), resultStr }
                });
                throw;
            }

            return resultStr;
        }

        public async Task<T> PostAsync<T, U>(string url, U data)
        {
            return JsonConvert.DeserializeObject<T>(await PostAsync(url, JsonConvert.SerializeObject(data)));
        }

        public async Task<string> PostAsync(string url, string request = null)
        {
            var resultStr = string.Empty;

            try
            {
                var response = await Policy
                    .Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => _httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                    .WaitAndRetryAsync(_appSettings.PollyMaxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (httpResponseMsg, attemptSeconds, context) =>
                    {
                        _logger.Log(LogLevel.Warning, httpResponseMsg?.Exception, $"{nameof(PostAsync)} failed, retrying:", new Dictionary<string, object>
                        {
                            { nameof(url), url},
                            { nameof(attemptSeconds), attemptSeconds },
                            { nameof(_appSettings.PollyMaxRetries), _appSettings.PollyMaxRetries },
                            { nameof(httpResponseMsg.Result.ReasonPhrase), httpResponseMsg?.Result?.ReasonPhrase },
                            { nameof(httpResponseMsg.Result.StatusCode), httpResponseMsg?.Result?.StatusCode }
                        });
                    })
                    .ExecuteAsync(() =>
                    {
                        var httpRequestMessage = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri($"{url}"),
                            Content = new StringContent(request, Encoding.UTF8, MediaTypeNames.Application.Json)
                        };

                        return _httpClient.SendAsync(httpRequestMessage);
                    });

                resultStr = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                ex.Data.Add("Result", resultStr);
                _logger.Log(LogLevel.Error, ex, $"{nameof(PostAsync)} failed:", new Dictionary<string, object>
                {
                    { nameof(url), url },
                    { nameof(resultStr), resultStr }
                });
                throw;
            }

            return resultStr;
        }
    }
}
