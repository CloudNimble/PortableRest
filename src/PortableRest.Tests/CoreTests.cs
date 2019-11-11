using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableRest.Tests.XmlTest;
using System;
using System.Net.Http;

namespace PortableRest.Tests
{

    [TestClass]
    public class CoreTests
    {
        /// <summary>
        /// Check the XML return from an object with attributes with all simple properties.
        /// </summary>
        [TestMethod]
        public void CheckMessageBodyXmlWithAttributes()
        {
            var expected = @"
							<PhoneNumber ID=""1"">
								 <Call />
								 <Calls />
								 <Number>514-9700</Number>
							</PhoneNumber>
						";
            var request = new RestRequest("/test?test1={test1}", HttpMethod.Get) { ContentType = ContentTypes.Xml };
            request.AddParameter(new PhoneNumber("1", "514-9700"));
            var body = request.GetRequestBody();
            Assert.IsNotNull(body);
            Assert.AreEqual(expected.AsXmlSanitized(), body.AsXmlSanitized());
        }

        /// <summary>
        /// Check the XML return from an object with attributes with 1 complex property that also has attributes.
        /// </summary>
        [TestMethod]
        public void CheckMessageBodyXmlWithAttributes2()
        {
            var expected = @"
							<PhoneNumber ID=""1"">
								 <Call ID=""1"">
										<Number>864-5789</Number>
										<Time>0001-01-01T00:00:00</Time>
								 </Call>
								 <Calls />
								 <Number>514-9700</Number>
							</PhoneNumber>
						";
            var request = new RestRequest("/test?test1={test1}", HttpMethod.Get) { ContentType = ContentTypes.Xml };
            var pn = new PhoneNumber("1", "514-9700")
            {
                Call = new PhoneCall
                {
                    ID = "1",
                    Number = "864-5789"
                }
            };
            request.AddParameter(pn);
            var body = request.GetRequestBody();
            Assert.IsNotNull(body);
            Assert.AreEqual(expected.AsXmlSanitized(), body.AsXmlSanitized());
        }

        /// <summary>
        /// Check the XML return from an object with attributes with 1 generic list property whose containing elements also have attributes.
        /// </summary>
        [TestMethod]
        public void CheckMessageBodyXmlWithAttributes3()
        {
            var expected = @"
              <PhoneNumber ID=""1"">
                 <Call />
                 <Calls>
                    <PhoneCall>
                       <Number>864-5789</Number>
                       <Time>0001-01-01T00:00:00</Time>
                    </PhoneCall>
                 </Calls>
                 <ID>1</ID>
                 <Number>514-9700</Number>
              </PhoneNumber>
            ";
            var request = new RestRequest("/test?test1={test1}", HttpMethod.Get) { ContentType = ContentTypes.Xml };
            var pn = new PhoneNumber("1", "514-9700");
            pn.Calls.Add(new PhoneCall { ID = "1", Number = "864-5789" });
            request.AddParameter(pn);
            var body = request.GetRequestBody();
            Assert.IsNotNull(body);

            Assert.AreEqual(expected.AsXmlSanitized(), body.AsXmlSanitized());
        }

        [TestMethod]
        public void RestResponseWithNullMessageShouldThrowError()
        {
            Action action = () => new RestResponse<string>(null, "Testing");
            action.Should().Throw<Exception>();
        }



        //private const string BaseAddress = "http://localhost:9385/";

        //[TestMethod]
        //public async Task CanAddAuthorizationHeaderWithoutError()
        //{
        //    // Setup
        //    var client = new RestClient { BaseUrl = BaseAddress };
        //    client.AddHeader("Authortization", );
        //    var request = new RestRequest("api/books");
        //    List<Book> response;

        //    // Execute
        //    using (WebApp.Start<WebApiStartup>(BaseAddress))
        //    {
        //        response = await client.ExecuteAsync<List<Book>>(request);
        //    }

        //    // Validate
        //    response.Should().NotBeNull();
        //    response.Count().Should().Be(5);
        //}



    }

}