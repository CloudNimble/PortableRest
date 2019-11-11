using CloudNimble.Breakdance.WebApi;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableRest.Tests.OwinSelfHostServer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortableRest.Tests
{
    [TestClass]
    public class ExecuteAsyncTests
    {

        [TestMethod]
        public async Task ExecuteAsyncOfTReturnsDeserializedContentOfT()
        {
            // Setup
            var server = WebApiTestHelpers.GetTestableHttpServer();
            var client = new RestClient(server) { BaseUrl = WebApiConstants.Localhost };
            var request = new RestRequest("api/books");
            var response = await client.ExecuteAsync<List<Book>>(request);

            // Validate
            response.Should().NotBeNull();
            response.Count.Should().Be(5);
        }

        [TestMethod]
        public async Task MultipleRequestsFromSameClientShouldNotFail()
        {
            // Setup
            var server = WebApiTestHelpers.GetTestableHttpServer();
            var client = new RestClient(server) { BaseUrl = WebApiConstants.Localhost };
            var request = new RestRequest("api/books");
            List<Book> response;

            // Execute
            response = await client.ExecuteAsync<List<Book>>(request);

            // Validate
            response.Should().NotBeNull();
            response.Count.Should().Be(5);

            var request2 = new RestRequest("api/books");
            List<Book> response2;

            // Execute
            response2 = await client.ExecuteAsync<List<Book>>(request2);

            // Validate
            response2.Should().NotBeNull();
            response2.Count.Should().Be(5);
        }

        /// <summary>
        /// For more info, please watch the video for correctly building asynchronous libraries in .NET
        //  at http://channel9.msdn.com/Events/TechEd/Europe/2013/DEV-B318#fbid=
        /// </summary>
        //[TestMethod]
        //public void AsyncLibrariesShouldNotDeadlockOnTaskResult()
        //{
        //    // Setup
        //    var client = new RestClient { BaseUrl = BaseAddress };
        //    var request = new RestRequest("api/books");
        //    List<Book> response = null;

        //    // Execute
        //    using (WebApp.Start<WebApiStartup>(BaseAddress))
        //    {
        //        // Simulate ASP.NET and Windows Forms thread affinity
        //        WindowsFormsContext.Run(() =>
        //        {
        //            // Should not deadlock on this call
        //            response = client.ExecuteAsync<List<Book>>(request).Result;
        //        });
        //    }

        //    // Validate
        //    Assert.IsTrue(true, "If we got to this assertion, then we didn't deadlock on the call to ExecuteAsync.");
        //    response.Should().NotBeNull();
        //    response.Count().Should().Be(5);
        //}
    }
}