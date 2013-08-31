using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Google.Discovery;

namespace PortableRest.Tests
{
    public class GoogleDiscoveryClient : RestClient
    {

        public string ApiVersion { get; set; }

        public GoogleDiscoveryClient()
        {

            ApiVersion = "v1";
            BaseUrl = "";

            // silverlight friendly way to get current version
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(assembly.FullName);
            var version = assemblyName.Version;

            BaseUrl = "https://www.googleapis.com/discovery/" + ApiVersion;
            UserAgent = "PortableRest Unit Test Client v" + version;

        }

        public async Task<ApisResult> GetApis()
        {
            var request = new RestRequest { Resource = "apis" };
            return await ExecuteAsync<ApisResult>(request);
        }


    }
}
