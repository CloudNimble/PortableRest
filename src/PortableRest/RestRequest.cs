using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

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
        private List<KeyValuePair<string, string>> UrlSegments { get; set; }

        private List<KeyValuePair<string, object>> Parameters { get; set; }

        //private List<KeyValuePair<string, string>> QueryString { get; set; }

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
            UrlSegments = new List<KeyValuePair<string, string>>();
            Parameters = new List<KeyValuePair<string, object>>();
            //QueryString = new List<KeyValuePair<string, string>>();
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
        /// Adds a parameter to the body of the request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>Note: If the ContentType is anything other than UrlFormEncoded, only the first Parameter will be serialzed to the request body.</remarks>
        public void AddParameter(string key, object value)
        {
            Parameters.Add(new KeyValuePair<string, object>(key, value));
        }

        ///// <summary>
        ///// Adds an item to the QueryString of the request.
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        //public void AddQueryString(string key, string value)
        //{
        //    QueryString.Add(new KeyValuePair<string, string>(key, value));
        //}

        /// <summary>
        /// Adds segments 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>This can be used for QueryString parameters too.</remarks>
        /// <example>Resource = "/Samples.aspx?Test1={test1}";</example>
        public void AddUrlSegment(string key, string value)
        {
            UrlSegments.Add(new KeyValuePair<string, string>(key, value));
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
            foreach (var segment in UrlSegments)
            {
                Resource = Resource.Replace("{" + segment.Key + "}", Uri.EscapeUriString(segment.Value));
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
                    throw new NotImplementedException("Sending XML is not yet supported, but will be added in a future release.");
                default:

                    return Parameters.Count > 0 ? JsonConvert.SerializeObject(Parameters[0].Value) : "";
            }
        }

        #endregion


    }
}
