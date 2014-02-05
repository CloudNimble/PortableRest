using System.Net.Http;

namespace PortableRest.Authentication
{
    /// <summary>
    ///     Represent a http authenticator
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        ///     Authenticate the request.
        /// </summary>
        /// <param name="client">The http client.</param>
        /// <param name="request">The request.</param>
        /// <param name="handler">The http request handler.</param>
        void Authenticate(IRestClient client, IRestRequest request, HttpClientHandler handler);
    }
}