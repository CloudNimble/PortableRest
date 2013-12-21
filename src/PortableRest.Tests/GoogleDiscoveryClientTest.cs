using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Discovery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PortableRest.Tests
{
    [TestClass]
    public class GoogleDiscoveryClientTest
    {
        [TestMethod]
        public async Task GetApisTest()
        {
            var client = new GoogleDiscoveryClient();
            var result = await client.GetApis();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ApisResult));
            Assert.IsInstanceOfType(result.Apis, typeof(List<Api>));
            Assert.IsNotNull(result.Apis[0]);
        }

        [TestMethod]
        public async Task GetApisWithNameTest()
        {
            var client = new GoogleDiscoveryClient();
            var result = await client.GetApis("adexchangebuyer");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ApisResult));
            Assert.IsInstanceOfType(result.Apis, typeof(List<Api>));
            Assert.IsNotNull(result.Apis[0]);
        }

        [TestMethod]
        public async Task GetApisRawTest()
        {
            var client = new GoogleDiscoveryClient();
            var result = await client.GetApisRaw("adexchangebuyer");
            Assert.AreNotEqual(result, "");
        }

    }
}
