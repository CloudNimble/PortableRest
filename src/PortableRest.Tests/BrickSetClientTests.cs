using System;
using System.Linq;
using System.Threading.Tasks;
using BrickSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PortableRest.Tests
{
    [TestClass]
    public class BrickSetClientTests
    {

        public TestContext TestContext { get; set; }

        private const string TestKey = "h6uj-0Csr-cMsJ";

        [Ignore]
        [TestMethod]
        public async Task GetThemesTest()
        {
            var client = new BrickSetClient(TestKey);
            var results = await client.GetThemes();
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count());
        }

        [Ignore]
        [TestMethod]
        public async Task GetAdditionalImagesTest()
        {
            var client = new BrickSetClient(TestKey);
            var results = await client.GetAdditionalImages(23345);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count());
        }


    }
}
