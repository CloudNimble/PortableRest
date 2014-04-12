using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableRest.Tests.AsyncTestUtilities;
using PortableRest.Tests.OwinSelfHostServer;

namespace PortableRest.Tests
{
    [TestClass]
    public class SendAsyncTests
    {
        private const string BaseAddress = "http://localhost:9385/";

        [TestMethod]
        public async Task NotFound404ReturnedWhenServerReturnsNotFoundHttpStatus()
        {
            // Setup
            var client = new RestClient{ BaseUrl = BaseAddress };
            var request = new RestRequest("notsuccess/notfound");
            RestResponse<string> response;

            // Execute
            using (WebApp.Start<WebApiStartup>(BaseAddress))
            {
                response = await client.SendAsync<string>(request);
            }

            // Validate
            response.HttpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Should().BeNull();
        }

        [TestMethod]
        public async Task InternalServerError500ReturnedWhenServerReturns500HttpStatus()
        {
            // Setup
            var client = new RestClient { BaseUrl = BaseAddress };
            var request = new RestRequest("notsuccess/internalservererror");
            RestResponse<string> response;

            // Execute
            using (WebApp.Start<WebApiStartup>(BaseAddress))
            {
                response = await client.SendAsync<string>(request);
            }

            // Validate
            response.HttpResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Content.Should().BeNull();
        }

        [TestMethod]
        public async Task SendAsyncOfTContainsHttpResponseAndDeserializedContent()
        {
            // Setup
            var client = new RestClient { BaseUrl = BaseAddress };
            var request = new RestRequest("api/books");
            RestResponse<IEnumerable<Book>> response;

            // Execute
            using (WebApp.Start<WebApiStartup>(BaseAddress))
            {
                response = await client.SendAsync<IEnumerable<Book>>(request);
            }

            // Validate
            response.HttpResponseMessage.Should().NotBeNull();
            response.HttpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();
            response.Content.Count().Should().Be(5);
        }

        /// <summary>
        /// For more info, please watch the video for correctly building asynchronous libraries in .NET
        //  at http://channel9.msdn.com/Events/TechEd/Europe/2013/DEV-B318#fbid=
        /// </summary>
        [TestMethod]
        public void async_libraries_like_portable_rest_should_not_deadlock_on_task_result()
        {
            // Setup
            var client = new RestClient { BaseUrl = BaseAddress };
            var request = new RestRequest("api/books");
            RestResponse<List<Book>> response = null;

            // Execute
            using (WebApp.Start<WebApiStartup>(BaseAddress))
            {
                // Simulate ASP.NET and Windows Forms thread affinity
                WindowsFormsContext.Run(() =>
                {
                    // Should not deadlock on this call
                    response = client.SendAsync<List<Book>>(request).Result;
                });
            }

            // Validate
            Assert.IsTrue(true, "If we got to this assertion, then we didn't deadlock on the call to SendAsync.");
            response.Content.Should().NotBeNull();
            response.Content.Count().Should().Be(5);
        }
    }
}