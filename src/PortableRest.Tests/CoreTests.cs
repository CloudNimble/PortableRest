using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableRest.Tests.XmlTest;

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
            var request = new RestRequest("/test?test1={test1}", HttpMethod.Get) {ContentType = ContentTypes.Xml};
            request.AddParameter(new PhoneNumber("1", "514-9700"));
            var body = request.GetRequestBody();
            Assert.IsNotNull(body);
            Assert.AreEqual("<PhoneNumber ID=\"1\">\r\n  <Call />\r\n  <Calls />\r\n  <Number>514-9700</Number>\r\n</PhoneNumber>", body);
        }

        /// <summary>
        /// Check the XML return from an object with attributes with 1 complex property that also has attributes.
        /// </summary>
        [TestMethod]
        public void CheckMessageBodyXmlWithAttributes2()
        {
            var request = new RestRequest("/test?test1={test1}", HttpMethod.Get) { ContentType = ContentTypes.Xml };
            var pn = new PhoneNumber("1", "514-9700") { Call = new PhoneCall
            {
                ID = "1", Number = "864-5789"
            }};
            request.AddParameter(pn);
            var body = request.GetRequestBody();
            Assert.IsNotNull(body);
            Assert.AreEqual("<PhoneNumber ID=\"1\">\r\n  <Call ID=\"1\">\r\n    <Number>864-5789</Number>\r\n    <Time>0001-01-01T00:00:00</Time>\r\n  </Call>\r\n  <Calls />\r\n  <Number>514-9700</Number>\r\n</PhoneNumber>", body);
        }

        /// <summary>
        /// Check the XML return from an object with attributes with 1 generic list property whose containing elements also have attributes.
        /// </summary>
        [TestMethod]
        public void CheckMessageBodyXmlWithAttributes3()
        {
            var request = new RestRequest("/test?test1={test1}", HttpMethod.Get) { ContentType = ContentTypes.Xml };
            var pn = new PhoneNumber("1", "514-9700");
            pn.Calls.Add(new PhoneCall{ID = "1", Number = "864-5789"});
            request.AddParameter(pn);
            var body = request.GetRequestBody();
            Assert.IsNotNull(body);
            Assert.AreEqual("<PhoneNumber ID=\"1\">\r\n  <Calls />\r\n  <Number>514-9700</Number>\r\n</PhoneNumber>", body);
        }
    }

}