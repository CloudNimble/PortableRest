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

        private HttpClient _client;

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
        /// 
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
            var url = restRequest.GetFormattedResource(BaseUrl);

            if (string.IsNullOrWhiteSpace(restRequest.DateFormat) && !string.IsNullOrWhiteSpace(DateFormat))
            {
                restRequest.DateFormat = DateFormat;
            }

            var handler = new HttpClientHandler {AllowAutoRedirect = true};

            _client = new HttpClient(handler);

            if (!string.IsNullOrWhiteSpace(UserAgent))
            {
                _client.DefaultRequestHeaders.Add("user-agent", UserAgent);
            }

            var message = new HttpRequestMessage(restRequest.Method, new Uri(restRequest.Resource, UriKind.RelativeOrAbsolute));

            foreach (var header in Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }

            if (restRequest.Method == HttpMethod.Post || restRequest.Method == HttpMethod.Put)
            {
                var contentString = new StringContent(restRequest.GetRequestBody(), Encoding.UTF8, restRequest.GetContentType());
                message.Content = contentString;
            }

            HttpResponseMessage response = null;
                response = await _client.SendAsync(message);
                response.EnsureSuccessStatusCode();       

            var responseContent = await response.Content.ReadAsStringAsync();

            //TODO: Handle Error
            if (response.Content.Headers.ContentType.MediaType == "application/xml")

            {
                #region Original plan - don't delete yet
                ////if (result is IEnumerable)
                ////{
                ////    xml.Replace("type=\"array\"", "xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\"");
                ////}

                //var originalXml = XElement.Parse(xml);
                //XElement node;
                //XNamespace jnk = "http://james.newtonking.com/projects/json";
                //var ns = new XAttribute(XNamespace.Xmlns + "json", "http://james.newtonking.com/projects/json");

                //if (restRequest.IgnoreRootElement)
                //{
                //    node = originalXml.Descendants().FirstOrDefault();
                //    if (node != null)
                //    {
                //        node.Add(ns);
                //    }
                //}
                //else
                //{
                //    node = originalXml;
                //    node.Add(ns);
                //}

                //foreach (var node in root.DescendantsAndSelf())
                //{
                //    foreach (XAttribute att in node.Attributes())
                //    {
                //        node.Add(new XElement(att.Name, (string) att));
                //    }
                //    node.Attributes().Remove();
                //}


                #endregion

                // RWM: IDEA - The DataContractSerializer doesn't like attributes, but will handle everything else.
                // So instead of trying to mess with a double-conversion to JSON and then to the Object, we'll just turn the attributes
                // into elements, and sort the elements into alphabetical order so the DataContracterializer doesn't crap itself.

                // On post, use a C# attribute to specify if a property is an XML attribute, DataContractSerialize to XML, then
                // query the object for [XmlAttribute] attributes and move them from elements to attributes using code similar to below.
                // If the POST request requires the attributes in a certain order, oh well. Shouldn't have used PHP :P.

                XElement root = XElement.Parse(responseContent);
                XElement newRoot = (XElement)Transform(restRequest.IgnoreRootElement ? root.Descendants().First() : root, restRequest);
                 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>Technique from http://blogs.msdn.com/b/ericwhite/archive/2009/07/20/a-tutorial-in-the-recursive-approach-to-pure-functional-transformations-of-xml.aspx</remarks>
        static object Transform(XNode node, RestRequest request)
        {
            XElement element = node as XElement;
            if (element != null)
            {
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
                        XElement e = n as XElement;
                        if (e != null)
                            return Transform(e, request);
                        return n;
                    }));
            }
            return node;
        }

    }
}
