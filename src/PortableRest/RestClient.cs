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
        private List<KeyValuePair<string, string>> Headers { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of the RestClient class.
        /// </summary>
        public RestClient()
        {
            Headers = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Adds a header for a given string key and string value.
        /// </summary>
        /// <param name="key">The header to add.</param>
        /// <param name="value">The value of the header being added.</param>
        public void AddHeader(string key, string value)
        {
            Headers.Add(new KeyValuePair<string, string>(key, value));
        }

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
