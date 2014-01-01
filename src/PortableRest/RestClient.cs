using System.Net;
using System.ServiceModel.Channels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PortableRest
{

    /// <summary>
    /// Base client to create REST requests and process REST responses.
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
        /// 
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// The User Agent string to pass back to the API.
        /// </summary>
        public string UserAgent { get; set; }

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
        public async Task<T> ExecuteAsync<T>(RestRequest restRequest) where T : class
        {
            T result = null;

            if (string.IsNullOrWhiteSpace(restRequest.DateFormat) && !string.IsNullOrWhiteSpace(DateFormat))
            {
                restRequest.DateFormat = DateFormat;
            }

            var handler = new HttpClientHandler {AllowAutoRedirect = true};
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            _client = new HttpClient(handler);

            if (!string.IsNullOrWhiteSpace(UserAgent))
            {
                _client.DefaultRequestHeaders.Add("user-agent", UserAgent);
            }

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

            HttpResponseMessage response = null;
            response = await _client.SendAsync(message);
            response.EnsureSuccessStatusCode();       

            var responseContent = await response.Content.ReadAsStringAsync();

            if (restRequest.ReturnRawString)
            {
                result = responseContent as T;
            }
            else if (response.Content.Headers.ContentType.MediaType == "application/xml")
            {

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
                    var settings = new XmlReaderSettings {IgnoreWhitespace = true};
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
            }
            else
            {
                result = JsonConvert.DeserializeObject<T>(responseContent);
            }

            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
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

        #endregion

    }
}
