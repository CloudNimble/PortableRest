using System;
using System.Net.Http;
using JetBrains.Annotations;

namespace PortableRest
{
    /// <summary>
    /// Represents a HTTP response message that may contain deserialized data of type T
    /// if the response was successful.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the response.</typeparam>
    public sealed class RestResponse<T> : IDisposable where T : class
    {

        #region Properties

        /// <summary>
        /// Gets the content from the response.
        /// </summary>
        /// <returns>
        /// If the response does not indicate success then returns null.
        /// If T is <see cref="string"/> then returns the raw string content without being deserialized.
        /// Otherwise returns the deserialized response content of T.
        /// </returns>
        public T Content { get; private set; }

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        public HttpResponseMessage HttpResponseMessage { get; private set; }

        /// <summary>
        /// The exception that was thrown during object de-serializtion.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResponse{T}"/> class.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response. Cannot be null.</param>
        /// <param name="content">The deserialized content from the response.</param>
        public RestResponse([NotNull] HttpResponseMessage httpResponseMessage, T content)
        {
            if (httpResponseMessage == null)
            {
                throw new ArgumentNullException("httpResponseMessage");
            }

            HttpResponseMessage = httpResponseMessage;
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResponse{T}"/> class.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response. Cannot be null.</param>
        /// <param name="content">The deserialized content from the response.</param>
        /// <param name="serializationException">The exception that was thrown during object de-serializtion.</param>
        public RestResponse(HttpResponseMessage httpResponseMessage, T content, Exception serializationException) : this(httpResponseMessage, content)
        {
            Exception = serializationException;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            HttpResponseMessage.Dispose();
        }

        #endregion

    }
}