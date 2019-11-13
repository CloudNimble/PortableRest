using System;
using System.Linq;
using System.Threading.Tasks;
using BrickSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PortableRest.Tests
{

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class BrickSetClientTests
    {

        /// <summary>
        /// 
        /// </summary>
        public TestContext TestContext { get; set; }

        private const string TestKey = "h6uj-0Csr-cMsJ";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Ignore]
        [TestMethod]
        public async Task GetThemesTest()
        {
            var client = new BrickSetClient(TestKey);
            var results = await client.GetThemes().ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Ignore]
        [TestMethod]
        public async Task GetAdditionalImagesTest()
        {
            var client = new BrickSetClient(TestKey);
            var results = await client.GetAdditionalImages(23345).ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.Count());
        }


    }
}
