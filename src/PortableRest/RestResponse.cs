using System;
using System.Net.Http;

namespace PortableRest
{
    /// <summary>
    /// Represents a HTTP response message that may contain deserialized data of type T
    /// if the response was successful.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the response.</typeparam>
    public sealed class RestResponse<T> : IDisposable 
        where T : class
    {
        private readonly HttpResponseMessage _httpResponseMessage;
        private readonly T _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResponse{T}"/> class.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response. Cannot be null.</param>
        /// <param name="content">The deserialized content from the response.</param>
        public RestResponse(HttpResponseMessage httpResponseMessage, T content)
        {
            if (httpResponseMessage == null)
            {
                throw new ArgumentNullException("httpResponseMessage");
            }

            _httpResponseMessage = httpResponseMessage;
            _content = content;
        }

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage
        {
            get { return _httpResponseMessage; }
        }

        /// <summary>
        /// Gets the content from the response.
        /// </summary>
        /// <returns>
        /// If the response does not indicate success then returns null.
        /// If T is <see cref="string"/> then returns the raw string content without being deserialized.
        /// Otherwise returns the deserialized response content of T.
        /// </returns>
        public T Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _httpResponseMessage.Dispose();
        }
    }
}