using System;
using System.Net.Http;

namespace PortableRest
{
    /// <summary>
    /// Exception containing the HTTP response, in case of an unsuccessfull status code.
    /// </summary>
    public class HttpResponseException : Exception
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public HttpResponseMessage Response { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public HttpResponseException(HttpResponseMessage response)
            : base("Response status code does not indicate success.")
        {
            if(response == null) throw new ArgumentNullException("response");
            Response = response;
        }
    }
}