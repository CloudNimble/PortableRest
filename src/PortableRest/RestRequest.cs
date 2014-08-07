using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PortableRest
{

    /// <summary>
    /// Specifies the parameters for the HTTP request that will be executed against a given resource.
    /// </summary>
    public class RestRequest
    {

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private List<UrlSegment> UrlSegments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal List<EncodedParameter> Parameters { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="ContentType"/> of the request.
        /// </summary>
        public ContentTypes ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> Headers { get; set; }

        /// <summary>
        /// Specifies whether or not the root element in the response.
        /// </summary>
        public bool IgnoreRootElement { get; set; }

        /// <summary>
        /// When <see cref="ContentTypes.Xml"/>, specifies whether or not attributes should be ignored.
        /// </summary>
        public bool IgnoreXmlAttributes { get; set; }

        /// <summary>
        /// Allows you to have more control over how JSON content is serialized to the request body.
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// The HTTP method to use for the request.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// A string representation of the specific resource to access, using ASP.NET MVC-like replaceable tokens.
        /// </summary>
        public string Resource { internal get; set; }

        /// <summary>
        /// Tells the RestClient to skip deserialization and return the raw result.
        /// </summary>
        [Obsolete("ReturnRawString is deprecated, please just specify RestClient.ExecuteAsync<string> instead.")]
        public bool ReturnRawString { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new RestRequest instance, assuming the request will be an HTTP GET.
        /// </summary>
        public RestRequest()
        {
            UrlSegments = new List<UrlSegment>();
            Parameters = new List<EncodedParameter>();
            Headers = new Dictionary<string, object>();
            Method = HttpMethod.Get;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource.
        /// </summary>
        /// <param name="resource">The specific resource to access.</param>
        public RestRequest(string resource)
            : this()
        {
            Resource = resource;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource and Method, specifying whether or not to ignore the root object in the response.
        /// </summary>
        /// <param name="resource">The URL format string of the resource to request.</param>
        /// <param name="contentType">The <see cref="ContentTypes">Content Type</see> for the request.</param>
        public RestRequest(string resource, ContentTypes contentType)
            : this(resource)
        {
            ContentType = contentType;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource and Method.
        /// </summary>
        /// <param name="resource">The specific resource to access.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        public RestRequest(string resource, HttpMethod method)
            : this(resource)
        {
            Method = method;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource and Method, specifying whether or not to ignore the root object in the response.
        /// </summary>
        /// <param name="resource">The URL format string of the resource to request.</param>
        /// <param name="method">The <see cref="HttpMethod"/> for the request.</param>
        /// <param name="ignoreRoot">Whether or not the root object from the response should be ignored.</param>
        public RestRequest(string resource, HttpMethod method, bool ignoreRoot)
            : this(resource, method)
        {
            IgnoreRootElement = ignoreRoot;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource and Method, specifying whether or not to ignore the root object in the response.
        /// </summary>
        /// <param name="resource">The URL format string of the resource to request.</param>
        /// <param name="method">The <see cref="HttpMethod"/> for the request.</param>
        /// <param name="contentType">The <see cref="ContentTypes">Content Type</see> for the request.</param>
        public RestRequest(string resource, HttpMethod method, ContentTypes contentType)
            : this(resource, method)
        {
            ContentType = contentType;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an Header to only this specific request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>Use this if you have an authentication token that times out on a regular basis.</remarks>
        public void AddHeader(string key, object value)
        {
            Headers.Add(key, value);
        }

        /// <summary>
        /// Adds an unnamed parameter to the body of the request.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>Use this method if you're not using UrlFormEncoded requests.</remarks>
        public void AddParameter(object value)
        {
            AddParameter("", value);
        }

        /// <summary>
        /// Adds a parameter to the body of the request, to be encoded with the specified format.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">
        /// For ByteArrays or Base64, this needs to be a Stream, or an exception will be thrown when the request is executed.
        /// </param>
        /// <param name="encoding"></param>
        /// <remarks>Note: If the ContentType is anything other than UrlFormEncoded, only the first Parameter will be serialzed to the request body.</remarks>
        public void AddParameter(string key, object value, ParameterEncoding encoding = ParameterEncoding.UriEncoded)
        {
            Parameters.Add(new EncodedParameter(key, value, encoding));
        }


        /// <summary>
        /// Replaces tokenized segments of the URL with a desired value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <example>If <code>Resource = "{entity}/Samples.aspx"</code> and <code>someVariable.Publisher = "Disney";</code>, then
        /// <code>Resource.AddUrlSegment("entity", someVariable.Publisher);</code> becomes <code>Resource = "Disney/Samples.aspx";</code></example>
        public void AddUrlSegment(string key, string value)
        {
            UrlSegments.Add(new UrlSegment(key, value));
        }

        /// <summary>
        /// Appends a key/value pair to the end of the existing QueryString in a URI.
        /// </summary>
        /// <param name="key">The string key to append to the QueryString.</param>
        /// <param name="value">The string value to append to the QueryString.</param>
        public void AddQueryString(string key, string value)
        {
            UrlSegments.Add(new UrlSegment(key, value, true));
        }

        /// <summary>
        /// Appends a key/value pair to the end of the existing QueryString in a URI.
        /// </summary>
        /// <param name="key">The string key to append to the QueryString.</param>
        /// <param name="value">The value to append to the QueryString (we will call .ToString() for you).</param>
        public void AddQueryString(string key, object value)
        {
            AddQueryString(key, value.ToString());
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        internal Uri GetResourceUri(string baseUrl)
        {
            foreach (var segment in UrlSegments.Where(c => !c.IsQueryString))
            {
                Resource = Resource.Replace("{" + segment.Key + "}", Uri.EscapeUriString(segment.Value));
            }

            if (UrlSegments.Any(c => c.IsQueryString))
            {
                var queryString = UrlSegments.Where(c => c.IsQueryString)
                    .Aggregate(new StringBuilder(),
                        (current, next) =>
                            current.Append(string.Format("&{0}={1}", Uri.EscapeUriString(next.Key), Uri.EscapeDataString(next.Value))))
                    .ToString();

                Resource = string.Format(Resource.Contains("?") ? "{0}{1}" : "{0}?{1}", Resource, queryString);
            }

            Resource = CombineUriParts(baseUrl, Resource);

            return new Uri(Resource, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Combines URI parts, taking care of trailing and starting slashes.
        /// See http://stackoverflow.com/a/6704287
        /// </summary>
        /// <param name="uriParts">The URI parts to combine.</param>
        private static string CombineUriParts(params string[] uriParts)
        {
            var uri = string.Empty;
            if (uriParts != null && uriParts.Any())
            {
                uriParts = uriParts.Where(part => !string.IsNullOrWhiteSpace(part)).ToArray();
                char[] trimChars = { '\\', '/' };
                uri = (uriParts[0] ?? string.Empty).TrimEnd(trimChars);
                for (var i = 1; i < uriParts.Count(); i++)
                {
                    uri = string.Format("{0}/{1}", uri.TrimEnd(trimChars), (uriParts[i] ?? string.Empty).TrimStart(trimChars));
                }
            }
            return uri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal string GetContentType()
        {
            switch (ContentType)
            {
                case ContentTypes.FormUrlEncoded:
                    return "application/x-www-form-urlencoded";
                case ContentTypes.Xml:
                    return "application/xml";
                default:
                    return "application/json";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal string GetRequestBody()
        {
            switch (ContentType)
            {
                case ContentTypes.FormUrlEncoded:
                    var parameters = Parameters.Aggregate(new StringBuilder(),
                        (current, next) =>
                            current.Append(string.Format("{0}{1}={2}", current.Length > 0 ? "&" : "",
                                Uri.EscapeDataString(next.Key),
                                next.GetEncodedValue())));
                    return parameters.ToString();

                case ContentTypes.Xml:
                    var result = "";
                    if (Parameters.Count == 0) return result;

                    var type = Parameters[0].Value.GetType();

                    var serializer = new DataContractSerializer(type);
                    using (var stream = new MemoryStream())
                    {
                        serializer.WriteObject(stream, Parameters[0].Value);
                        result = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
                    }

                    if (IgnoreXmlAttributes || string.IsNullOrWhiteSpace(result)) return result;

                    var doc = XElement.Parse(result);
                    //Clean all of the DataContract namespaces from the payload.
                    doc.Attributes().Remove();
                    Transform(IgnoreRootElement ? doc.Descendants().First() : doc, Parameters[0].Value.GetType());
                    return doc.ToString();
                default:
                    switch (Parameters.Count)
                    {
                        case 0:
                            return "";
                        case 1:
                            return JsonConvert.SerializeObject(Parameters[0].Value, JsonSerializerSettings);
                        default:
                            var body = new JObject();
                            foreach (var parameter in Parameters)
                            {
                                body.Add(parameter.Key, new JObject(parameter.Value));
                            }
                            return JsonConvert.SerializeObject(body, JsonSerializerSettings);
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>Technique from http://blogs.msdn.com/b/ericwhite/archive/2009/07/20/a-tutorial-in-the-recursive-approach-to-pure-functional-transformations-of-xml.aspx </remarks>
        private static void Transform(XNode node, Type type)
        {
            var element = node as XElement;
            if (element == null) return;
            element.Attributes().Remove();

            //var t = type.GetTypeInfo();

            //RWM: Do the recursion first, so matching elements in child objects don't accidentally get picked up early.

            //TODO: Handle generic lists
            //foreach (var prop in t.DeclaredProperties.Where(c => !(c.PropertyType.GetTypeInfo().IsSimpleType())))
            foreach (var prop in type.GetProperties().Where(c => !(c.PropertyType.IsSimpleType())))
            {
                Debug.WriteLine(prop.Name);
                var xnode = element.Descendants().FirstOrDefault(c => c.Name.ToString() == prop.Name);
                if (xnode != null)
                {
                    Transform(xnode, prop.PropertyType);
                }
            }

            //foreach (var prop in t.DeclaredProperties.Where(c => c.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any()))
            foreach (var prop in type.GetProperties().Where(c => c.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any()))
            {
                var attribs = prop.GetCustomAttributes(true);
                if (attribs.Any(c => c is IgnoreDataMemberAttribute || c is XmlIgnoreAttribute)) continue;

                var xnode = element.Descendants().FirstOrDefault(c => c.Name.ToString() == prop.Name);
                if (xnode == null) continue;
                element.SetAttributeValue(xnode.Name, xnode.Value);
                xnode.Remove();
            }

            //TODO: RWM: Handle time formats properly.
            //foreach (var prop in t.DeclaredProperties.Where(c => c.PropertyType == typeof (DateTime)))
            //{
            //    var xnode = element.Descendants().FirstOrDefault(c => c.Name.ToString() == prop.Name);
            //    if (xnode == null) continue;
            //    var newValue = DateTime.ParseExact(element.Value, DateFormat, null);
            //    element.Value = XmlConvert.ToString(newValue);
            //}

        }

        #endregion
    }
}
