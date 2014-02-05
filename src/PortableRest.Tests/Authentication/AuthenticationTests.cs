using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PortableRest.Authentication;

namespace PortableRest.Tests.Authentication
{
    [TestClass]
    public class AuthenticationTests : MockingRestClientTest
    {
        [TestMethod, TestCategory("Authentication")]
        public async Task RestClient_Call_Authenticate_Once()
        {
            // Arrange
            var mockAuthenticator = new Mock<IAuthenticator>();
            RestClient client = MockRestClient();
            client.Authenticator = mockAuthenticator.Object;

            // Act
            await client.ExecuteAsync<EmptyResponse>(new EmptyRequest());

            // Assert
            mockAuthenticator.Verify(x => x.Authenticate(It.IsAny<IRestClient>(), It.IsAny<RestRequest>(), It.IsAny<HttpClientHandler>()), Times.Once());
        }

        [TestMethod, TestCategory("Authentication")]
        public async Task Rest_Client_Throws_If_Authenticator_Throws()
        {
            // Arrange
            var mockAuthenticator = new Mock<IAuthenticator>();
            mockAuthenticator.Setup(x => x.Authenticate(It.IsAny<IRestClient>(), It.IsAny<RestRequest>(), It.IsAny<HttpClientHandler>()))
                .Throws(new Exception());
            
            bool exceptionThrown = false;

            RestClient client = MockRestClient();
            client.Authenticator = mockAuthenticator.Object;

            // Act
            try
            {
                await client.ExecuteAsync<EmptyResponse>(new EmptyRequest());
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown, "Rest Client catches exception occurred during authentication process");
        }
    }
}