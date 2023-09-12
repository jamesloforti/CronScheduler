using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using WebServiceDefault.Common.Models;
using WebServiceDefault.Library.Clients.Interfaces;
using WebServiceDefault.Library.Providers;

namespace WebServiceDefault.Library.Tests.Providers
{
    [TestFixture]
    public class ExampleProviderTests
    {
        private Mock<ILogger<ExampleProvider>> _logger;
        private Mock<IGenericHttpClient> _client;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<ExampleProvider>>();
            _client = new Mock<IGenericHttpClient>();
        }

        [Test, AutoData]
        public void SendAsync_Happy(ExampleRequest request)
        {
            _client.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync("result");
            var provider = new ExampleProvider(_logger.Object, _client.Object);
            var actual = provider.SendAsync(request).Result;
            Assert.IsFalse(string.IsNullOrEmpty(actual));
        }
    }
}
