namespace PortableRest
{
    /// <summary>
    /// Specifies the parameters for the HTTP POST request that will be executed against a given resource.
    /// </summary>
    /// <typeparam name="TBody">The type of the body of the request</typeparam>
    public class RestPostRequest<TBody> : RestRequest where TBody : class
    {
        public RestPostRequest()
        {
            Method = "POST";
        }

        public RestPostRequest(string resource) : base(resource, "POST")
        {
        }

        public RestPostRequest(string resource, bool ignoreRoot) : base(resource, "POST", ignoreRoot)
        {
        }

        // TODO: Implement XML serialization
        ///// <summary>
        ///// If true the body is sent as XML, otherwise as JSON.
        ///// </summary>
        //public bool XmlContentType { get; set; }

        /// <summary>
        /// The body of the request.
        /// </summary>
        public TBody Body { get; set; }
    }
}
