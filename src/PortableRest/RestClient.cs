using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PortableRest
{

    /// <summary>
    /// Base client to create REST requests and process REST responses. Uses <see cref="HttpClient"/> as the underlying transport.
    /// </summary>
    public class RestClient : IDisposable
    {

        #region Private Members

        private static HttpClient _client;
        private HttpMessageHandler _httpHandler;

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
        /// <remarks>If you do not set this value, it will be set for you by calling SetUserAgent() before the request is executed.</remarks>
        public string UserAgent { get; set; }

        /// <summary>
        /// A shared <see cref="CookieContainer"/> that will be used for all requests.
        /// </summary>
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// The internal HttpMessageHandler to use for the request. 
        /// </summary>
        /// <remarks>
        /// The HttpMessageHandler will be configured for our purposes immediately after being set.
        /// </remarks>
        public HttpMessageHandler HttpHandler
        {
            get { return _httpHandler; }
            set
            {
                _httpHandler = value;
                ConfigureHandler(_httpHandler);
            }
        }

        /// <summary>
        /// Allows you to have more control over how JSON content is serialized to the request body.
        /// </summary>
        /// <contributor>https://github.com/jeffijoe</contributor>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// A list of KeyValuePairs that will be appended to the Headers collection for all requests.
        /// </summary>
        private List<KeyValuePair<string, string>> Headers { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the RestClient class.
        /// </summary>
        public RestClient()
        {
            Headers = new List<KeyValuePair<string, string>>();
            CookieContainer = new CookieContainer();
            HttpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            _client = new HttpClient(HttpHandler);
        }

        /// <summary>
        /// Creates a new instance of the RestClient class.
        /// </summary>
        /// <param name="handler">The HttpMessageHandler instance to use for all requests with this RestClient.</param>
        public RestClient([NotNull] HttpMessageHandler handler)
        {
            Headers = new List<KeyValuePair<string, string>>();
            CookieContainer = new CookieContainer();
            HttpHandler = handler;
            _client = new HttpClient(HttpHandler);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a header for a given string key and string value.
        /// </summary>
        /// <param name="key">The header to add.</param>
        /// <param name="value">The value of the header being added.</param>
        public void AddHeader([NotNull] string key, string value)
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
            var thisAssembly = typeof(T).GetTypeInfo().Assembly;
            var thisAssemblyName = new AssemblyName(thisAssembly.FullName);
            var thisVersion = thisAssemblyName.Version;

            if (displayName == null)
            {
                var attributes = thisAssembly.GetCustomAttributes<AssemblyTitleAttribute>().ToList();
                if (attributes.Count() == 0)
                {
                    throw new Exception("The assembly containing the class inheriting from PortableRest.RestClient must have an AssemblyTitle attribute specified.");
                }
                displayName = attributes[0].Title;
            }

            var prAssembly = typeof(RestRequest).GetTypeInfo().Assembly;
            var prAssemblyName = new AssemblyName(prAssembly.FullName);
            var prVersion = prAssemblyName.Version;

            UserAgent = string.Format("{0} {1} (PortableRest {2})", displayName, thisVersion.ToString(3), prVersion.ToString(3));
        }

        /// <summary>
        /// Executes an asynchronous request to the given resource and deserializes the response content to an object of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An object of T.</returns>
        /// <exception cref="HttpRequestException">
        /// Throws an exception if the <see cref="HttpResponseMessage.IsSuccessStatusCode"/> property for the HTTP response is false.
        /// </exception>
        public async Task<T> ExecuteAsync<T>([NotNull] RestRequest restRequest, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            var httpResponseMessage = await GetHttpResponseMessage<T>(restRequest, cancellationToken).ConfigureAwait(false);

            httpResponseMessage.EnsureSuccessStatusCode();

            return await GetResponseContent<T>(restRequest, httpResponseMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes an asynchronous request to the given resource and returns a RestResponse
        /// which contains the <see cref="HttpResponseMessage" /> and the response content of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize from the content.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="PortableRestException">This type of exception is thrown when an error happens either before a request has started,
        /// or after it has finished and the result is being processed.</exception>
        public async Task<RestResponse<T>> SendAsync<T>([NotNull] RestRequest restRequest, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await GetHttpResponseMessage<T>(restRequest, cancellationToken).ConfigureAwait(false);
                var content = await GetResponseContent<T>(restRequest, httpResponseMessage).ConfigureAwait(false);
                return new RestResponse<T>(httpResponseMessage, content);
            }
            //RWM: Added 22 Oct 2015.
            //     If we caught an exception lower on the stack, make sure it's bubbled up.
            catch (PortableRestException prEx)
            {
                throw prEx;
            }
            catch (Exception ex)
            {
                return new RestResponse<T>(new HttpResponseMessage(HttpStatusCode.BadRequest), null, ex);
            }
        }

        /// <summary>
        /// Disposes of the resources used by RestClient.
        /// </summary>
        public void Dispose()
        {
            HttpHandler.Dispose();
            _client.Dispose();
        }

#endregion

#region Private Methods

        /// <summary>
        /// Configures the HttpMessageHandler to ensure requests can be compressed and use the specified CookieContainer.
        /// </summary>
        /// <param name="handler">The HttpMessageHandler to configure.</param>
        private void ConfigureHandler(HttpMessageHandler handler)
        {
            if (handler == null)
            {
                throw new PortableRestException("Could not find an HttpClientHandler instance to configure. Please check to make sure that any custom HttpMessageHandler " +
                                                "passed into the RestClient constructor create a new instace of HttpClientHandler at the base of its DelegatingHandler chain.");
            }
            //RWM: This Handler could be a part of a chain of handlers. Recursion!
            var delegatingHandler = handler as DelegatingHandler;
            if (delegatingHandler != null)
            {
                ConfigureHandler(delegatingHandler.InnerHandler);
            }

            //RWM: We can't do anything if we get down the chain and we don't have an HttpClientHandler, so bail.
            if (!(handler is HttpClientHandler)) return;

            var clientHandler = ((HttpClientHandler)handler);
            clientHandler.AllowAutoRedirect = true;
            if (clientHandler.SupportsAutomaticDecompression)
            {
                clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            if (CookieContainer != null)
            {
                clientHandler.CookieContainer = CookieContainer;
            }
        }

        /// <summary>
        /// Helps deal with the fact that the XmlSerializer is not supported, and the DataContractSerializer hates XmlAttributes.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>Technique from http://blogs.msdn.com/b/ericwhite/archive/2009/07/20/a-tutorial-in-the-recursive-approach-to-pure-functional-transformations-of-xml.aspx </remarks>
        private static object Transform(XNode node, [NotNull] RestRequest request)
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

            return new XElement(XNamespace.None.GetName(element.Name.LocalName),
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetHttpResponseMessage<T>([NotNull] RestRequest restRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            //RWM If we've specified a DateFormat for the Client, but not not the Request, pass it down.
            if (!string.IsNullOrWhiteSpace(DateFormat) && string.IsNullOrWhiteSpace(restRequest.DateFormat))
            {
                restRequest.DateFormat = DateFormat;
            }

            //RWM: If we've specified JsonSerializerSettings for the Client, but not not the Request, pass it down.
            if (JsonSerializerSettings != null && restRequest.JsonSerializerSettings == null)
            {
                restRequest.JsonSerializerSettings = JsonSerializerSettings;
            }

            if (string.IsNullOrWhiteSpace(UserAgent))
            {
                SetUserAgent<T>();
            }

            //RWM: We likely only need to set this once.
            if (!_client.DefaultRequestHeaders.UserAgent.Any())
            {
                _client.DefaultRequestHeaders.Add("user-agent", UserAgent);
            }

            var message = new HttpRequestMessage(restRequest.Method, restRequest.GetResourceUri(BaseUrl));

            //RWM: Add the global headers for all requests.
            foreach (var header in Headers)
            {
                message.Headers.TryAddWithoutValidation(header.Key, header.Value);
                //message.Headers.Add(header.Key, header.Value);
            }

            //RWM: Add request-specific headers.
            foreach (var header in restRequest.Headers)
            {
                message.Headers.TryAddWithoutValidation(header.Key, header.Value.ToString());
                //message.Headers.Add(header.Key, header.Value.ToString());
            }

            //RWM: Not sure if this is sufficient, or if HEAD supports a body, will need to check into the RFC.
            if (restRequest.Method != HttpMethod.Get && restRequest.Method != HttpMethod.Head && restRequest.Method != HttpMethod.Trace)
            {
                //RWM: This feels hacky. May need some tweaking.
                if (restRequest.ContentType == ContentTypes.ByteArray)
                {
                    //RWM: A fix for an issue uncovered by @scottisafool.
                    if (restRequest.Parameters.Count > 0)
                    {
                        message.Content = new ByteArrayContent(restRequest.Parameters[0].GetEncodedValue() as byte[]);
                    }
                }
                //RWM: This may not be the best place to keep this... might be better to refactor RestRequest.GetRequestBody to return a HttpContent object instead.
                else if (restRequest.ContentType == ContentTypes.MultiPartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach (var p in restRequest.Parameters)
                    {
                        if (p is FileParameter)
                        {
                            var fileParameter = p as FileParameter;
                            if (string.IsNullOrEmpty(fileParameter.Filename))
                            {
                                content.Add(new StreamContent(fileParameter.Value as Stream), fileParameter.Key);
                            }
                            else
                            {
                                content.Add(new StreamContent(fileParameter.Value as Stream), fileParameter.Key, fileParameter.Filename);
                            }
                        }
                        else if (p.Encoding == ParameterEncoding.ByteArray)
                        {
                            content.Add(new ByteArrayContent(p.GetEncodedValue() as byte[]));
                        }
                        else
                        {
                            content.Add(new StringContent(p.GetEncodedValue().ToString()), p.Key);
                        }
                    }

                    message.Content = content;
                }
                else
                {
                    var contentString = new StringContent(restRequest.GetRequestBody(), Encoding.UTF8, restRequest.GetContentType());
                    message.Content = contentString;
                }
            }

            return await _client.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest"></param>
        /// <param name="httpResponseMessage"></param>
        /// <returns></returns>
        private static async Task<T> GetResponseContent<T>([NotNull] RestRequest restRequest, HttpResponseMessage httpResponseMessage) where T : class
        {
            var rawResponseContent = await GetRawResponseContent(httpResponseMessage).ConfigureAwait(false);
            if (rawResponseContent == null) return null;
            // ReSharper disable once CSharpWarnings::CS0618
            if (typeof(T) == typeof(string) || restRequest.ReturnRawString)
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
        private static async Task<string> GetRawResponseContent([NotNull] HttpResponseMessage response)
        {
            //RWM: Explicitly check for NoContent... because the request was successful but there is nothing to do.
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
        private static T DeserializeResponseContent<T>([NotNull] RestRequest restRequest, [NotNull] HttpResponseMessage response, string responseContent) where T : class
        {

            switch (response.Content.Headers.ContentType.MediaType)
            {
                case "application/xml":
                case "text/xml":
                    return DeserializeApplicationXml<T>(restRequest, responseContent);
                //TODO: RWM: Figure out how to parse returned files.
                //case "multipart/form-data":

                //TODO: Handle more response types... like files.
                default:
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseContent, restRequest.JsonSerializerSettings);
                    }
                    catch (JsonSerializationException jEx)
                    {
                        throw new PortableRestException("The JsonConverter failed. Please see InnerException for details.", jEx);
                    }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest"></param>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        private static T DeserializeApplicationXml<T>([NotNull] RestRequest restRequest, string responseContent) where T : class
        {
            T result;
            // RWM: IDEA - The DataContractSerializer doesn't like attributes, but will handle everything else.
            // So instead of trying to mess with a double-conversion to JSON and then to the Object, we'll just turn the attributes
            // into elements, and sort the elements into alphabetical order so the DataContracterializer doesn't crap itself.

            // On post, use a C# attribute to specify if a property is an XML attribute, DataContractSerialize to XML, then
            // query the object for [XmlAttribute] attributes and move them from elements to attributes using code similar to below.
            // If the POST request requires the attributes in a certain order, oh well. Shouldn't have used PHP :P.

            var root = XElement.Parse(responseContent);
            var newRoot = (XElement)Transform(restRequest.IgnoreRootElement ? root.Descendants().First() : root, restRequest);

            using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(newRoot.ToString())))
            {
                var settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true,
                };
                using (var reader = XmlReader.Create(memoryStream, settings))
                {
                    try
                    {
                        var serializer = new DataContractSerializer(typeof(T));
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
