using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace PortableRest.Tests.Authentication
{
    public abstract class MockingRestClientTest
    {
        internal class EmptyResponse
        {
        }

        internal class EmptyRequest : RestRequest
        {
            public EmptyRequest()
                : base("/SomeNotNullPathToAvoidException")
            { }
        }

        public static RestClient MockRestClient()
        {
            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) });

            var clientMock = new Mock<RestClient>();

            var mockHttpClient = new Mock<IInternalHttpClient>();
            mockHttpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns(tcs.Task);

            mockHttpClient.SetupGet(x => x.DefaultRequestHeaders)
                .Returns((HttpRequestHeaders)
                    typeof(HttpRequestHeaders).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(new object[] { }));

            clientMock
                .Protected()
                .Setup<IInternalHttpClient>("CreateHttpClient", ItExpr.IsAny<HttpClientHandler>())
                .Returns(mockHttpClient.Object);

            clientMock
                .Protected()
                .Setup<HttpClientHandler>("CreateHttpClientHandler")
                .Returns(new HttpClientHandler {AllowAutoRedirect = true});

            return clientMock.Object;
        }
    }
}