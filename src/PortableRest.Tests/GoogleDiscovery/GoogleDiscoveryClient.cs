using System.Reflection;
using System.Threading.Tasks;
using Google.Discovery;

namespace PortableRest.Tests
{

    public class GoogleDiscoveryClient : RestClient
    {

        /// <summary>
        /// The version of the API to access. This version is typically embedded into the URL of the request, 
        /// although occasionally you can specify it in the header. We support both options.
        /// </summary>
        /// <remarks>
        /// You can change the logic on the getter to automatically update the BaseUrl if the ApiVersion changes.
        /// </remarks>
        public string ApiVersion { get; set; }


        /// <summary>
        /// The default constructor for the GoogleDiscoveryClient.
        /// </summary>
        /// <remarks>
        /// This would usually be the place where you can pass in API keys or other credentials.
        /// </remarks>
        public GoogleDiscoveryClient()
        {
            // The new PCL + .NET 4.5 way to get the assembly through Reflection/
            var assembly = typeof(ApisResult).GetTypeInfo().Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);
            var version = assemblyName.Version;

            UserAgent = "PortableRest Unit Test Client v" + version;
            ApiVersion = "v1";
            BaseUrl = "https://www.googleapis.com/discovery/" + ApiVersion;
        }

        /// <summary>
        /// Gets a list of APIs that are available from Google to develop against.
        /// </summary>
        /// <returns>An ApiResult object containing the APIs available from Google.</returns>
        public async Task<ApisResult> GetApis(string name = "")
        {
            var request = new RestRequest("apis");
            if (!string.IsNullOrWhiteSpace(name))
            {
                request.AddQueryString("name", name);
            }
            return await ExecuteAsync<ApisResult>(request);
        }

    }

}