using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Xml;
using System.Linq;

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

        private List<KeyValuePair<string, object>> Parameters { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ContentTypes ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Specifies whether or not the root 
        /// </summary>
        public bool IgnoreRootElement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreXmlAttributes { get; set; }

        /// <summary>
        /// The HTTP method to use for the request.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// A string representation of the specific resource to access, using ASP.NET MVC-like replaceable tokens.
        /// </summary>
        public string Resource { internal get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new RestRequest instance, assuming the request will be an HTTP GET.
        /// </summary>
        public RestRequest()
        {
            UrlSegments = new List<UrlSegment>();
            Parameters = new List<KeyValuePair<string, object>>();
            Method = HttpMethod.Get;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource.
        /// </summary>
        /// <param name="resource"></param>
        public RestRequest(string resource) : this()
        {
            Resource = resource;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource and Method.
        /// </summary>
        /// <param name="resource">The specific resource to access.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        public RestRequest(string resource, HttpMethod method) : this(resource)
        {
            Method = method;
        }

        /// <summary>
        /// Creates a new RestRequest instance for a given Resource and Method, specifying whether or not to ignore the root object in the response.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <param name="ignoreRoot"></param>
        public RestRequest(string resource, HttpMethod method, bool ignoreRoot) : this(resource, method)
        {
            IgnoreRootElement = ignoreRoot;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an unnamed parameter to the body of the request.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>Use this method if you're not using UrlFormEncoded requests.</remarks>
        public void AddParameter(object value)
        {
            Parameters.Add(new KeyValuePair<string, object>("", value));
        }

        /// <summary>
        /// Adds a parameter to the body of the request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>Note: If the ContentType is anything other than UrlFormEncoded, only the first Parameter will be serialzed to the request body.</remarks>
        public void AddParameter(string key, object value)
        {
            Parameters.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Adds segments 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>This can be used for QueryString parameters too.</remarks>
        /// <example>Resource = "/Samples.aspx?Test1={test1}";</example>
        public void AddUrlSegment(string key, string value)
        {
            UrlSegments.Add(new UrlSegment(key, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddQueryString(string key, string value)
        {
            UrlSegments.Add(new UrlSegment(key, value, true));
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
                            current.Append(string.Format("&{0}={1}", next.Key, Uri.EscapeUriString(next.Value))))
                    .ToString();

                Resource = string.Format(Resource.Contains("?") ? "{0}{1}" : "{0}?{1}", Resource, queryString);
            }

            if (!string.IsNullOrEmpty(Resource) && Resource.StartsWith("/"))
            {
                Resource = Resource.Substring(1);
            }

            if (!string.IsNullOrEmpty(baseUrl))
            {
                Resource = string.IsNullOrEmpty(Resource) ? baseUrl : string.Format("{0}/{1}", baseUrl, Resource);
            }
            return new Uri(Resource, UriKind.RelativeOrAbsolute);
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
                    var parameters = Parameters.Aggregate("", (s, pair) =>
                                                  s + string.Format("{0}{1}={2}", s.Length > 0 ? "&" : "",
                                                                Uri.EscapeDataString(pair.Key),
                                                                Uri.EscapeDataString(pair.Value.ToString())));
                    return parameters;

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
                    return Parameters.Count > 0 ? JsonConvert.SerializeObject(Parameters[0].Value) : "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks>Technique from http://blogs.msdn.com/b/ericwhite/archive/2009/07/20/a-tutorial-in-the-recursive-approach-to-pure-functional-transformations-of-xml.aspx </remarks>
        private void Transform(XNode node, Type type)
        {
            var element = node as XElement;
            if (element == null) return;
            element.Attributes().Remove();

            var t = type.GetTypeInfo();

            //RWM: Do the recursion first, so matching elements in child objects don't accidentally get picked up early.

            //TODO: Handle generic lists
            foreach (var prop in t.DeclaredProperties.Where(c => !(c.PropertyType.GetTypeInfo().IsSimpleType())))
            {
                Debug.WriteLine(prop.Name);
                var xnode = element.Descendants().FirstOrDefault(c => c.Name.ToString() == prop.Name);
                if (xnode != null)
                {
                    Transform(xnode, prop.PropertyType);
                }

            }

            foreach (var prop in t.DeclaredProperties.Where(c => c.GetCustomAttributes<XmlAttributeAttribute>().Any()))
            {
                var attribs = prop.GetCustomAttributes();
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
