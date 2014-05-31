using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableRest.Tests.AsyncTestUtilities;
using PortableRest.Tests.OwinSelfHostServer;

namespace PortableRest.Tests
{
    [TestClass]
    public class ExecuteAsyncTests
    {
        private const string BaseAddress = "http://localhost:9385/";

        [TestMethod]
        public async Task ExecuteAsyncOfTReturnsDeserializedContentOfT()
        {
            // Setup
            var client = new RestClient { BaseUrl = BaseAddress };
            var request = new RestRequest("api/books");
            List<Book> response;

            // Execute
            using (WebApp.Start<WebApiStartup>(BaseAddress))
            {
                response = await client.ExecuteAsync<List<Book>>(request);
            }

            // Validate
            response.Should().NotBeNull();
            response.Count().Should().Be(5);
        }

        /// <summary>
        /// For more info, please watch the video for correctly building asynchronous libraries in .NET
        //  at http://channel9.msdn.com/Events/TechEd/Europe/2013/DEV-B318#fbid=
        /// </summary>
        [TestMethod]
        public void AsyncLibrariesShouldNotDeadlockOnTaskResult()
        {
            // Setup
            var client = new RestClient { BaseUrl = BaseAddress };
            var request = new RestRequest("api/books");
            List<Book> response = null;

            // Execute
            using (WebApp.Start<WebApiStartup>(BaseAddress))
            {
                // Simulate ASP.NET and Windows Forms thread affinity
                WindowsFormsContext.Run(() =>
                {
                    // Should not deadlock on this call
                    response = client.ExecuteAsync<List<Book>>(request).Result;
                });
            }

            // Validate
            Assert.IsTrue(true, "If we got to this assertion, then we didn't deadlock on the call to ExecuteAsync.");
            response.Should().NotBeNull();
            response.Count().Should().Be(5);
        }
    }
}