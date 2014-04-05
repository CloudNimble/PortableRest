using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace PortableRest
{

    /// <summary>
    /// Base client to create REST requests and process REST responses. Uses <see cref="HttpClient"/> as the underlying transport.
    /// </summary>
    public class RestClient
    {

        #region Private Members

        private HttpClient _client;

        #endregion

        #region Properties

        /// <summary>
        /// The base URL for the resource this client will access.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The format to be used when serializing and deserializing dates.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// The User Agent string to pass back to the API.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// A shared <see cref="CookieContainer"/> that will be used for all requests.
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// A list of KeyValuePairs that will be appended to the Headers collection for all requests.
        /// </summary>
        private List<KeyValuePair<string, string>> Headers { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of the RestClient class.
        /// </summary>
        public RestClient()
        {
            Headers = new List<KeyValuePair<string, string>>();
            CookieContainer = new CookieContainer();
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
        /// Sets the <see cref="UserAgent"/> for this client in a standardized format using a Type from your client library.
        /// </summary>
        /// <param name="displayName">
        /// Optional. The name you want displayed for this Client. If left blank, it will default to the AssemblyTitleAttribute value from the AssemblyInfo file.
        /// </param>
        /// <typeparam name="T">A type from your Client Library that can be used to get the assembly information.</typeparam>
        /// <remarks>This will set the <see cref="UserAgent"/> to "YourAssemblyName Major.Minor.Revision (PortableRest Major.Minor.Revision)</remarks>
        public void SetUserAgent<T>(string displayName = null)
        {
            var thisAssembly = typeof (T).Assembly;
            var thisAssemblyName = new AssemblyName(thisAssembly.FullName);
            var thisVersion = thisAssemblyName.Version;

            if (displayName == null)
            {
                var attributes = thisAssembly.GetCustomAttributes(typeof (AssemblyTitleAttribute), false);
                if (attributes.Length == 0) 
                {
                    throw new Exception("The assembly containing the class inheriting from PortableRest.RestClient must have an AssemblyTitle attribute specified.");
                }
                displayName = ((AssemblyTitleAttribute)attributes[0]).Title;
            }

            var prAssembly = typeof(RestRequest).Assembly;
            var prAssemblyName = new AssemblyName(prAssembly.FullName);
            var prVersion = prAssemblyName.Version;

            UserAgent = string.Format("{0} {1} (PortableRest {2})", displayName, thisVersion.ToString(3), prVersion.ToString(3));
        }

        /// <summary>
        /// Executes an asynchronous request to the given resource and deserializes the response content to an object of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <returns>An object of T.</returns>
        /// <exception cref="HttpRequestException">
        /// Throws an exception if the <see cref="HttpResponseMessage.IsSuccessStatusCode"/> property for the HTTP response is false.
        /// </exception>
        public async Task<T> ExecuteAsync<T>(RestRequest restRequest) where T : class
        {
            var httpResponseMessage = await GetHttpResponseMessage<T>(restRequest);

            httpResponseMessage.EnsureSuccessStatusCode();

            return await GetResponseContent<T>(restRequest, httpResponseMessage);
        }

        /// <summary>
        /// Executes an asynchronous request to the given resource and returns a RestResponse
        /// which contains the <see cref="HttpResponseMessage"/> and the response content of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the content.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <exception cref="PortableRestException">
        /// This type of exception is thrown when an error happens either before a request has started, 
        /// or after it has finished and the result is being processed.
        /// </exception>
        public async Task<RestResponse<T>> SendAsync<T>(RestRequest restRequest) where T : class
        {
            var httpResponseMessage = await GetHttpResponseMessage<T>(restRequest);

            var content = await GetResponseContent<T>(restRequest, httpResponseMessage);

            return new RestResponse<T>(httpResponseMessage, content);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helps deal with the fact that the XmlSerializer is not supported, and the DataContractSerializer hates XmlAttributes.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>Technique from http://blogs.msdn.com/b/ericwhite/archive/2009/07/20/a-tutorial-in-the-recursive-approach-to-pure-functional-transformations-of-xml.aspx </remarks>
        private static object Transform(XNode node, RestRequest request)
        {
            var element = node as XElement;
            if (element == null) return node;

            if (!request.IgnoreXmlAttributes)
            {
                foreach (var attrib in element.Attributes())
                {
                    element.Add(new XElement(attrib.Name, (string)attrib));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.DateFormat) && 
                (element.Name.LocalName.ToLower().Contains("date") ||
                 element.Name.LocalName.ToLower().Contains("time")))
            {
                var newValue = DateTime.ParseExact(element.Value, request.DateFormat, null);
                element.Value = XmlConvert.ToString(newValue);
            }

            //RWM: NOTE the DataContractSerializer does not like null nodes when parsing nullable numbers.
            //However removing empty nodes seems to work.
            if (!element.Nodes().Any()) return null;

            return new XElement(element.Name,
                element.Nodes()
                    .OrderBy(c => (c as XElement) != null ? (c as XElement).Name.LocalName : c.ToString())
                    .Select(n =>
                    {
                        var e = n as XElement;
                        return e != null ? Transform(e, request) : n;
                    }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetHttpResponseMessage<T>(RestRequest restRequest)
        {
            if (string.IsNullOrWhiteSpace(restRequest.DateFormat) && !string.IsNullOrWhiteSpace(DateFormat))
            {
                restRequest.DateFormat = DateFormat;
            }

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };

            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            if (CookieContainer != null)
            {
                handler.CookieContainer = CookieContainer;
            }

            _client = new HttpClient(handler);

            if (string.IsNullOrWhiteSpace(UserAgent))
            {
                SetUserAgent<T>();
            }
            _client.DefaultRequestHeaders.Add("user-agent", UserAgent);

            var message = new HttpRequestMessage(restRequest.Method, restRequest.GetResourceUri(BaseUrl));

            //RWM: Add the global headers for all requests.
            foreach (var header in Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            //RWM: Add request-specific headers.
            foreach (var header in restRequest.Headers)
            {
                message.Headers.Add(header.Key, header.Value.ToString());
            }

            //RWM: Not sure if this is sufficient, or if HEAD supports a body, will need to check into the RFC.
            if (restRequest.Method != HttpMethod.Get && restRequest.Method != HttpMethod.Head && restRequest.Method != HttpMethod.Trace)
            {
                //REM: This feels hacky. May need some tweaking.
                if (restRequest.ContentType == ContentTypes.ByteArray)
                {
                    message.Content = new ByteArrayContent(restRequest.Parameters[0].GetEncodedValue() as byte[]);
                }
                else
                {
                    var contentString = new StringContent(restRequest.GetRequestBody(), Encoding.UTF8, restRequest.GetContentType());
                    message.Content = contentString;
                }
            }

            return await _client.SendAsync(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest"></param>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        private static async Task<T> GetResponseContent<T>(RestRequest restRequest, HttpResponseMessage httpResponseMessage) where T : class
        {
            var rawResponseContent = await GetRawResponseContent(httpResponseMessage);

            // ReSharper disable once CSharpWarnings::CS0618
            if (typeof (T) == typeof (string) || restRequest.ReturnRawString)
            {
                return rawResponseContent as T;
            }

            return DeserializeResponseContent<T>(restRequest, httpResponseMessage, rawResponseContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static async Task<string> GetRawResponseContent(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest"></param>
        /// <param name="response"></param>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        private static T DeserializeResponseContent<T>(RestRequest restRequest, HttpResponseMessage response, string responseContent) where T : class
        {
            if (response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                return DeserializeApplicationXml<T>(restRequest, responseContent);
            }
            //TODO: Handle more response types... like files.
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest"></param>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        private static T DeserializeApplicationXml<T>(RestRequest restRequest, string responseContent) where T : class
        {
            T result;
            // RWM: IDEA - The DataContractSerializer doesn't like attributes, but will handle everything else.
            // So instead of trying to mess with a double-conversion to JSON and then to the Object, we'll just turn the attributes
            // into elements, and sort the elements into alphabetical order so the DataContracterializer doesn't crap itself.

            // On post, use a C# attribute to specify if a property is an XML attribute, DataContractSerialize to XML, then
            // query the object for [XmlAttribute] attributes and move them from elements to attributes using code similar to below.
            // If the POST request requires the attributes in a certain order, oh well. Shouldn't have used PHP :P.

            var root = XElement.Parse(responseContent);
            var newRoot = (XElement) Transform(restRequest.IgnoreRootElement ? root.Descendants().First() : root, restRequest);

            using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(newRoot.ToString())))
            {
                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true
                };
                using (var reader = XmlReader.Create(memoryStream, settings))
                {
                    try
                    {
                        var serializer = new DataContractSerializer(typeof (T));
                        result = serializer.ReadObject(reader) as T;
                    }
                    catch (SerializationException ex)
                    {
                        throw new PortableRestException(string.Format("The serializer failed on node '{0}'", reader.Name), reader.Name, ex);
                    }
                }
            }
            return result;
        }

        #endregion

    }
}
