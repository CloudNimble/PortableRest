using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableRest.Authentication;

namespace PortableRest.Tests.Authentication
{
    [TestClass]
    public class BasicAuthenticationTests : MockingRestClientTest
    {
        private const string AuthorizationHeader = "Authorization";

        [TestMethod, TestCategory("Authentication")]
        public async Task Authorization_Header_Is_Present()
        {
            // Arrange
            var client = MockRestClient();
            client.Authenticator = new BasicAuthenticator("", "");

            var request = new EmptyRequest();

            Debug.Assert(request.Headers.Count == 0);

            // Act
            await client.ExecuteAsync<EmptyResponse>(request);

            // Assert
            Assert.IsTrue(client.Headers.Any(e => e.Key.Equals(AuthorizationHeader, StringComparison.OrdinalIgnoreCase)), "Authorization header was not added by " + client.Authenticator.GetType().Name);
        }

        [TestMethod, TestCategory("Authentication")]
        public async Task Authorization_Header_Is_Well_Formed()
        {
            // Arrange
            const string username = "u";
            const string password = "p";

            // http://en.wikipedia.org/wiki/Basic_access_authentication#Client_side
            string expectedToken = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))));

            var client = MockRestClient();
            client.Authenticator = new BasicAuthenticator(username, password);

            var request = new EmptyRequest();

            // Act
            await client.ExecuteAsync<EmptyResponse>(request);

            // Assert
            object generatedToken = client.Headers.Single(kvp => kvp.Key.Equals(AuthorizationHeader, StringComparison.OrdinalIgnoreCase)).Value;

            Assert.AreEqual(expectedToken, generatedToken, "BasicAuthenticator does not create a valid Authorization header");
        }
    }
}