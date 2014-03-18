using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}