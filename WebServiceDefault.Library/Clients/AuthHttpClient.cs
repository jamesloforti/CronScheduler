using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebServiceDefault.Common.Models.Authentication;
using WebServiceDefault.Common.Settings;
using WebServiceDefault.Library.Clients.Interfaces;

namespace WebServiceDefault.Library.Clients
{
    public class AuthHttpClient : ISimpleAuthClient<AuthHttpClient>
    {
        private readonly ILogger<AuthHttpClient> _logger;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;
        private readonly AuthSettings _authSettings;
        private readonly HttpStatusCode[] _httpStatusCodesWorthRetrying;
        private string _accessToken;

        public string AccessToken => _accessToken;

        public AuthHttpClient(ILogger<AuthHttpClient> logger, HttpClient httpClient, AuthSettings authSettings, AppSettings appSettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _authSettings = authSettings;
            _httpStatusCodesWorthRetrying = new HttpStatusCode[]
            {
                HttpStatusCode.RequestTimeout, // 408
                HttpStatusCode.InternalServerError, // 500
                HttpStatusCode.BadGateway, // 502
                HttpStatusCode.ServiceUnavailable, // 503
                HttpStatusCode.GatewayTimeout // 504
            };
            _appSettings = appSettings;
        }

        public async Task AuthenticateAsync()
        {
            string rawResponse = string.Empty;
            AuthResponse result = null;

            _logger.Log(LogLevel.Information, $"Attempting to {nameof(AuthenticateAsync)}.");

            try
            {
                var content = CreateFormContent();

                var response = await Policy
                    .Handle<HttpRequestException>()
                    .OrResult<HttpResponseMessage>(r => _httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                    .WaitAndRetryAsync(_appSettings.PollyMaxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (httpResponseMsg, attemptSeconds, context) =>
                    {
                        _logger.Log(LogLevel.Warning, httpResponseMsg?.Exception, $"Failed to {nameof(AuthenticateAsync)}, retrying:", new Dictionary<string, object>
                        {
                            { nameof(attemptSeconds), attemptSeconds.ToString() },
                            { nameof(_appSettings.PollyMaxRetries), _appSettings.PollyMaxRetries.ToString() },
                            { nameof(httpResponseMsg.Result.ReasonPhrase), httpResponseMsg?.Result?.ReasonPhrase },
                            { nameof(httpResponseMsg.Result.StatusCode), httpResponseMsg?.Result?.StatusCode }
                        });
                    })
                    .ExecuteAsync(() =>
                    {
                        var httpRequestMessage = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri($"{_authSettings.AuthUrl}/token"),
                            Content = content
                        };
                        return _httpClient.SendAsync(httpRequestMessage);
                    });

                rawResponse = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<AuthResponse>(rawResponse);
                _accessToken = result.Access_Token;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, $"Failed to {nameof(AuthenticateAsync)}:", new Dictionary<string, object>
                {
                    { nameof(rawResponse), rawResponse }
                });
                ex.Data.Add(nameof(rawResponse), rawResponse);
                throw;
            }
        }

        private FormUrlEncodedContent CreateFormContent()
        {
            var request = new AuthRequest
            {
                grant_type = "password",
                client_id = _authSettings.ClientId,
                client_secret = _authSettings.ClientSecret,
                username = _authSettings.Username,
                password = _authSettings.Password + _authSettings.SecurityToken
            };
            return new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(nameof(request.grant_type), request.grant_type),
                new KeyValuePair<string, string>(nameof(request.client_id), request.client_id),
                new KeyValuePair<string, string>(nameof(request.client_secret), request.client_secret),
                new KeyValuePair<string, string>(nameof(request.username), request.username),
                new KeyValuePair<string, string>(nameof(request.password), request.password),
            });
        }
    }

}
