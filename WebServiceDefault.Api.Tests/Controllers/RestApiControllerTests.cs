using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using WebServiceDefault.Api.Controllers;
using WebServiceDefault.Common.Models;
using WebServiceDefault.Library.Providers.Interfaces;

namespace WebServiceDefault.Api.Tests.Controllers
{
    [TestFixture]
    public class RestApiControllerTests
    {
        private Mock<ILogger<RestApiController>> _logger;
        private Mock<IExampleProvider> _provider;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<RestApiController>>();
            _provider = new Mock<IExampleProvider>();
        }

        [Test, AutoData]
        public void SendAsync_Status201Created_Happy(ExampleRequest request)
        {
            _provider.Setup(x => x.SendAsync(It.IsAny<ExampleRequest>())).ReturnsAsync("OK");
            var controller = new RestApiController(_logger.Object, _provider.Object);
            var actual = controller.SendAsync(request).Result as OkObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, actual.StatusCode);
        }

        [Test, AutoData]
        public void SendAsync_Status400BadRequest_Sad(ExampleRequest request)
        {
            _provider.Setup(x => x.SendAsync(It.IsAny<ExampleRequest>())).Throws(new HttpRequestException());
            var controller = new RestApiController(_logger.Object, _provider.Object);
            var actual = controller.SendAsync(request).Result as BadRequestObjectResult;
            Assert.AreEqual(StatusCodes.Status400BadRequest, actual.StatusCode);
        }

        [Test, AutoData]
        public void SendAsync_Status500InternalServerError_Sad(ExampleRequest request)
        {
            _provider.Setup(x => x.SendAsync(It.IsAny<ExampleRequest>())).Throws(new Exception());
            var controller = new RestApiController(_logger.Object, _provider.Object);
            var actual = controller.SendAsync(request).Result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }

        [Test, AutoData]
        public void SendAsync_Status500InternalServerError_ExceptionData_Sad(ExampleRequest request)
        {
            var ex = new Exception("test");
            ex.Data.Add("test", "stuff");
            _provider.Setup(x => x.SendAsync(It.IsAny<ExampleRequest>())).Throws(ex);
            var controller = new RestApiController(_logger.Object, _provider.Object);
            var actual = controller.SendAsync(request).Result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actual.StatusCode);
        }
    }
}
