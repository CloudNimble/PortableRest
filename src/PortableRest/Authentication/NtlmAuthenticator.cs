using System;
using System.Net;
using System.Net.Http;

namespace PortableRest.Authentication
{
    /// <summary>
    ///     Tries to Authenticate with the given credentials, or impersonate a user
    /// </summary>
    public class NtlmAuthenticator : IAuthenticator
    {
        private readonly ICredentials _credentials;

        /// <summary>
        ///     Authenticate by impersonation
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public NtlmAuthenticator(string username, string password)
            : this(new NetworkCredential(username, password))
        {
        }

        /// <summary>
        ///     Authenticate by impersonation, using an existing <c>ICredentials</c> instance
        /// </summary>
        /// <param name="credentials"></param>
        public NtlmAuthenticator(ICredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            _credentials = credentials;
        }

        /// <summary>
        ///     Authenticate the request.
        /// </summary>
        /// <param name="client">The http client.</param>
        /// <param name="request">The request.</param>
        /// <param name="handler">The http request handler.</param>
        public void Authenticate(IRestClient client, IRestRequest request, HttpClientHandler handler)
        {
            handler.AllowAutoRedirect = true;
            handler.Credentials = _credentials;
        }
    }
}