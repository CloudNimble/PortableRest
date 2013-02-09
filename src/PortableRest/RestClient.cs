using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


namespace PortableRest
{

    /// <summary>
    /// Base client to create REST requests and process REST responses.
    /// </summary>
    public class RestClient
    {

        #region Properties

        /// <summary>
        /// The base URL for the resource this client will access.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// A list of KeyValuePairs that will be appended to the Headers collection for all requests.
        /// </summary>
        public List<KeyValuePair<HttpRequestHeader, string>> Headers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Executes an asynchronous request to the given resource and deserializes the response to an object of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <returns>An object of T.</returns>
        public async Task<T> ExecuteAsync<T>(RestRequest restRequest)
        {
            T result = default(T);
            var url = restRequest.GetFormattedResource(BaseUrl);

            var request = WebRequest.Create(url);
            request.Method = restRequest.Method;

            foreach (var header in Headers)
            {
                request.Headers[header.Key] = header.Value;
            }

            var response = await request.GetResponseAsync();
            if (response.ContentLength > 0)
            {
                result = JsonConvert.DeserializeObject<T>(response.ReadResponseStream());
            }
            
            return result;
        }

        #endregion

    }
}
