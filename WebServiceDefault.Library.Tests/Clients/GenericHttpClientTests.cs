using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebServiceDefault.Common.Settings;
using WebServiceDefault.Library.Clients;
using WebServiceDefault.Library.Tests.Clients;

namespace WebServiceDefault.Library.Tests.Providers
{
    [TestFixture]
    public class GenericHttpClientTests
    {
        private Mock<ILogger<GenericHttpClient>> _logger;
        private AppSettings _appSettings;
        private Mock<HttpMessageHandler> _handler;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<GenericHttpClient>>();
            _appSettings = new AppSettings();
            _handler = new Mock<HttpMessageHandler>();
        }

        [Test]
        public async Task GetAsync_Happy()
        {
            var url = "https://www.google.com";
            var response = "testing 1..2..3";
            _handler = MockSetup.SetupHttpMock(JsonConvert.SerializeObject(response));
            var httpClient = new HttpClient(_handler.Object);
            var client = new GenericHttpClient(_logger.Object, _appSettings, httpClient);
            var result = await client.GetAsync(url);

            Assert.IsTrue(MockSetup.Validate(_handler, 1, url, HttpMethod.Get));
            Assert.AreEqual(JsonConvert.DeserializeObject<string>(result), response);
        }

        [Test]
        public void GetAsync_Sad()
        {
            var url = "google.com";
            var response = "testing 1..2..3";
            _handler = MockSetup.SetupHttpMock(JsonConvert.SerializeObject(response));
            var httpClient = new HttpClient(_handler.Object);
            var client = new GenericHttpClient(_logger.Object, _appSettings, httpClient);
            Assert.ThrowsAsync<UriFormatException>(() => client.GetAsync(url));
        }

        [Test]
        public async Task PostAsync_Happy()
        {
            var url = "https://www.google.com";
            var response = "testing 1..2..3";
            var request = new DateTime();
            _handler = MockSetup.SetupHttpMock(JsonConvert.SerializeObject(response));
            var httpClient = new HttpClient(_handler.Object);
            var client = new GenericHttpClient(_logger.Object, _appSettings, httpClient);
            var result = await client.PostAsync(url, JsonConvert.SerializeObject(request));

            Assert.IsTrue(MockSetup.Validate(_handler, 1, url, HttpMethod.Post));
            Assert.AreEqual(JsonConvert.DeserializeObject<string>(result), response);
        }

        [Test]
        public void PostAsync_Sad()
        {
            var url = "google.com";
            var response = "testing 1..2..3";
            var request = new DateTime();
            _handler = MockSetup.SetupHttpMock(JsonConvert.SerializeObject(response));
            var httpClient = new HttpClient(_handler.Object);
            var client = new GenericHttpClient(_logger.Object, _appSettings, httpClient);
            Assert.ThrowsAsync<UriFormatException>(() => client.PostAsync(url, JsonConvert.SerializeObject(request)));
        }
    }

}
