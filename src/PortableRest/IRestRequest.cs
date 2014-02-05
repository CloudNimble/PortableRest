using System.Net.Http;

namespace PortableRest
{
    /// <summary>
    /// Represents a REST request.
    /// </summary>
    public interface IRestRequest
    {
        /// <summary>
        /// The <see cref="ContentType"/> of the request.
        /// </summary>
        ContentTypes ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string DateFormat { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //List<KeyValuePair<string, object>> Headers { get; set; }

        /// <summary>
        /// Specifies whether or not the root element in the response.
        /// </summary>
        bool IgnoreRootElement { get; set; }

        /// <summary>
        /// When <see cref="ContentTypes.Xml"/>, specifies whether or not attributes should be ignored.
        /// </summary>
        bool IgnoreXmlAttributes { get; set; }

        /// <summary>
        /// The HTTP method to use for the request.
        /// </summary>
        HttpMethod Method { get; set; }

        /// <summary>
        /// A string representation of the specific resource to access, using ASP.NET MVC-like replaceable tokens.
        /// </summary>
        string Resource { set; }

        /// <summary>
        /// Tells the RestClient to skip deserialization and return the raw result.
        /// </summary>
        bool ReturnRawString { get; set; }

        /// <summary>
        /// Adds an Header to only this specific request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>Use this if you have an authentication token that times out on a regular basis.</remarks>
        void AddHeader(string key, object value);

        /// <summary>
        /// Adds an unnamed parameter to the body of the request.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>Use this method if you're not using UrlFormEncoded requests.</remarks>
        void AddParameter(object value);

        /// <summary>
        /// Adds a parameter to the body of the request, to be encoded with the specified format.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">
        /// For ByteArrays or Base64, this needs to be a Stream, or an exception will be thrown when the request is executed.
        /// </param>
        /// <param name="encoding"></param>
        /// <remarks>Note: If the ContentType is anything other than UrlFormEncoded, only the first Parameter will be serialzed to the request body.</remarks>
        void AddParameter(string key, object value, ParameterEncoding encoding = ParameterEncoding.UriEncoded);

        /// <summary>
        /// Replaces tokenized segments of the URL with a desired value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <example>If <code>Resource = "{entity}/Samples.aspx"</code> and <code>someVariable.Publisher = "Disney";</code>, then
        /// <code>Resource.AddUrlSegment("entity", someVariable.Publisher);</code> becomes <code>Resource = "Disney/Samples.aspx";</code></example>
        void AddUrlSegment(string key, string value);

        /// <summary>
        /// Appends a key/value pair to the end of the existing QueryString in a URI.
        /// </summary>
        /// <param name="key">The string key to append to the QueryString.</param>
        /// <param name="value">The string value to append to the QueryString.</param>
        void AddQueryString(string key, string value);

        /// <summary>
        /// Appends a key/value pair to the end of the existing QueryString in a URI.
        /// </summary>
        /// <param name="key">The string key to append to the QueryString.</param>
        /// <param name="value">The value to append to the QueryString (we will call .ToString() for you).</param>
        void AddQueryString(string key, object value);
    }
}