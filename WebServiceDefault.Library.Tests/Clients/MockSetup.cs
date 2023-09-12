using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebServiceDefault.Library.Tests.Clients
{
    public static class MockSetup
    {
        public static Mock<HttpMessageHandler> SetupHttpMock(string responseContent)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                })
                .Verifiable();

            return handlerMock;
        }

        public static Mock<HttpMessageHandler> SetupHttpMockSad(string responseContent)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(responseContent)
                })
                .Verifiable();

            return handlerMock;
        }

        public static bool Validate(Mock<HttpMessageHandler> handler, int invocations, string path, HttpMethod method)
        {
            var expectedUri = new Uri($"{path}");

            try
            {
                handler.Protected().Verify
                (
                   "SendAsync",
                   Times.Exactly(invocations),
                   ItExpr.Is<HttpRequestMessage>
                   (
                       req => req.Method == method && req.RequestUri == expectedUri
                   ),
                   ItExpr.IsAny<CancellationToken>()
                );
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

}
