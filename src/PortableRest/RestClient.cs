using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

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
        /// 
        /// </summary>
        public string DateFormat { get; set; }

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
        /// <typeparam name="TReturn">The type to deserialize to.</typeparam>
        /// <typeparam name="TBody">The type of the body.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <returns>An object of T.</returns>
        public async Task<TReturn> ExecuteAsync<TReturn, TBody>(RestPostRequest<TBody> restRequest) where TReturn : class where TBody : class
        {
            var request = InternalExecuteAsync1(restRequest);

            if (request.Method == "POST")
            {
                request.ContentType = "application/json";
                string body = JsonConvert.SerializeObject(restRequest.Body);
                request.Headers[HttpRequestHeader.ContentLength] = body.Length.ToString();
                Stream requestStream = await request.GetRequestStreamAsync();
                using (StreamWriter streamWriter = new StreamWriter(requestStream))
                {
                    await streamWriter.WriteAsync(body);
                    await streamWriter.FlushAsync();
                }
            }

            var response = await InternalExecuteAsync2(request);

            return InternalExecuteAsync3<TReturn>(restRequest, response);
        }

        /// <summary>
        /// Executes an asynchronous request to the given resource and deserializes the response to an object of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <returns>An object of T.</returns>
        public async Task<T> ExecuteAsync<T>(RestRequest restRequest) where T : class
        {
            var request = InternalExecuteAsync1(restRequest);

            var response = await InternalExecuteAsync2(request);

            return InternalExecuteAsync3<T>(restRequest, response);
        }

        private static T InternalExecuteAsync3<T>(RestRequest restRequest, WebResponse response) where T : class
        {
            //TODO: Handle Error
            if (response.ContentType.Substring(0, response.ContentType.IndexOf(";")) == "application/xml")
            {
                var xml = response.ReadResponseStream();

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

                XElement root = XElement.Parse(xml);
                var newRoot = (XElement)Transform(root, restRequest.DateFormat);

                using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(newRoot.ToString())))
                {
                    using (var reader = XmlReader.Create(memoryStream))
                    {
                        var serializer = new DataContractSerializer(typeof(T));
                        return serializer.ReadObject(reader) as T;
                    }
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(response.ReadResponseStream());
            }
        }

        private async Task<WebResponse> InternalExecuteAsync2(WebRequest request)
        {
            foreach (var header in Headers)
            {
                request.Headers[header.Key] = header.Value;
            }

            var response = await request.GetResponseAsync();
            return response;
        }

        private WebRequest InternalExecuteAsync1(RestRequest restRequest)
        {
            var url = restRequest.GetFormattedResource(BaseUrl);

            if (string.IsNullOrWhiteSpace(restRequest.DateFormat) && !string.IsNullOrWhiteSpace(DateFormat))
            {
                restRequest.DateFormat = DateFormat;
            }

            var request = WebRequest.Create(url);
            request.Method = restRequest.Method;
            return request;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        static object Transform(XNode node, string dateFormat)
        {
            XElement element = node as XElement;
            if (element != null)
            {
                foreach (var attrib in element.Attributes())
                {
                    element.Add(new XElement(attrib.Name, (string)attrib));
                }
                element.RemoveAttributes();

                if (!string.IsNullOrWhiteSpace(dateFormat) && 
                    (element.Name.LocalName.ToLower().Contains("date") ||
                    element.Name.LocalName.ToLower().Contains("time")))
                {
                    var newValue = DateTime.ParseExact(element.Value, dateFormat, null);
                    element.Value = XmlConvert.ToString(newValue);
                }

                return new XElement(element.Name,
                    element.Nodes()
                    .OrderBy(c => (c as XElement) != null ? (c as XElement).Name.LocalName : c.ToString())
                    .Select(c => Transform(c, dateFormat)));
            }
            return node;
        }

    }
}
