using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PortableRest.Tests
{
    [TestClass]
    public class ResourceUriTests
    {
        [TestMethod]
        public void QueryStringReplacement()
        {
            var request = new RestRequest("/test?test1={test1}");
            request.AddUrlSegment("test1", "myValue");
            Assert.AreEqual("/test?test1=myValue", request.GetResourceUri("").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereBaseUriHasTrailingSlash()
        {
            var request = new RestRequest("test");
            Assert.AreEqual("http://test.com/test", request.GetResourceUri("http://test.com/").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereBaseUriDoesNotHaveTrailingSlash()
        {
            var request = new RestRequest("test");
            Assert.AreEqual("http://test.com/test", request.GetResourceUri("http://test.com").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereBaseUriAndResourceHaveSlashes()
        {
            var request = new RestRequest("/test");
            Assert.AreEqual("http://test.com/test", request.GetResourceUri("http://test.com/").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereResourceHasSlash()
        {
            var request = new RestRequest("/test");
            Assert.AreEqual("http://test.com/test", request.GetResourceUri("http://test.com").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereResourceIsNull()
        {
            var request = new RestRequest();
            Assert.AreEqual("http://test.com/", request.GetResourceUri("http://test.com").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereBaseUrlIsNull()
        {
            var request = new RestRequest("/test");
            Assert.AreEqual("/test", request.GetResourceUri(null).ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereResourceIsWhitespace()
        {
            var request = new RestRequest(" ");
            Assert.AreEqual("http://test.com/", request.GetResourceUri("http://test.com").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereResourceIsEmpty()
        {
            var request = new RestRequest(string.Empty);
            Assert.AreEqual("http://test.com/", request.GetResourceUri("http://test.com").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereBaseUriHasMultiplePathSegments()
        {
            var request = new RestRequest("apis");

            request.AddQueryString("name", "adexchangebuyer");

            Assert.AreEqual("https://www.googleapis.com/discovery/v1/apis?&name=adexchangebuyer", request.GetResourceUri("https://www.googleapis.com/discovery/v1").ToString());
        }

        [TestMethod]
        public void BuildResourceUriWhereResourceUriIsNull()
        {
            var request = new RestRequest();

            Assert.AreEqual("https://www.googleapis.com/discovery/v1/apis?&name=adexchangebuyer", 
                request.GetResourceUri("https://www.googleapis.com/discovery/v1/apis?&name=adexchangebuyer").ToString());
        }

        public void TestUriIdempodence()
        {
            var request = new RestRequest("/test");
            request.AddQueryString("test", "value");
            var originalResource = request.Resource;
            request.GetResourceUri("http://some.base.url");
            Assert.AreEqual(originalResource, request.Resource);
        }
    }
}