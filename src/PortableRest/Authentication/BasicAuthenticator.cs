using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PortableRest.Authentication
{
    /// <summary>
    ///     Represent a basic username and password authenticator.
    /// <seealso cref="http://tools.ietf.org/html/rfc2617"/>
    /// </summary>
    public class BasicAuthenticator : IAuthenticator
    {
        private const string AuthorizationHeaderName = "Authorization";

        private readonly string _password;
        private readonly string _username;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasicAuthenticator" />.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password</param>
        public BasicAuthenticator(string username, string password)
        {
            _username = username;
            _password = password;
        }

        /// <summary>
        ///     Authenticate the request.
        /// </summary>
        /// <param name="client">The http client.</param>
        /// <param name="request">The request.</param>
        /// <param name="handler">The http request handler.</param>
        public void Authenticate(IRestClient client, IRestRequest request, HttpClientHandler handler)
        {
            //if (!request.Headers.Any(h => h.Key.Equals(AuthorizationHeaderName, StringComparison.OrdinalIgnoreCase)))
            //{
                string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _username, _password)));
                string authHeader = string.Format("Basic {0}", token);
                
                client.AddHeader(AuthorizationHeaderName, authHeader);
            //}
        }
    }
}