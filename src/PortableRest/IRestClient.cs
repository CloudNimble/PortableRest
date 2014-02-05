using System.Net;
using System.Threading.Tasks;
using PortableRest.Authentication;

namespace PortableRest
{
    /// <summary>
    ///     Abstracts a REST client.
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        ///     The base URL for the resource this client will access.
        /// </summary>
        string BaseUrl { get; set; }

        /// <summary>
        ///     The format to be used when serializing and deserializing dates.
        /// </summary>
        string DateFormat { get; set; }

        /// <summary>
        ///     The User Agent string to pass back to the API.
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        ///     The authenticator used for authenticating the request.
        ///     <remarks>
        ///         Can be null.
        ///     </remarks>
        /// </summary>
        IAuthenticator Authenticator { get; set; }

        /// <summary>
        ///     A shared <see cref="CookieContainer" /> that will be used for all requests.
        /// </summary>
        CookieContainer CookieContainer { get; }

        /// <summary>
        ///     Adds a header for a given string key and string value.
        /// </summary>
        /// <param name="key">The header to add.</param>
        /// <param name="value">The value of the header being added.</param>
        void AddHeader(string key, string value);

        /// <summary>
        ///     Sets the <see cref="RestClient.UserAgent" /> for this client in a standardized format using a Type from your client
        ///     library.
        /// </summary>
        /// <param name="displayName">
        ///     Optional. The name you want displayed for this Client. If left blank, it will default to the AssemblyTitleAttribute
        ///     value from the AssemblyInfo file.
        /// </param>
        /// <typeparam name="T">A type from your Client Library that can be used to get the assembly information.</typeparam>
        /// <remarks>
        ///     This will set the <see cref="RestClient.UserAgent" /> to "YourAssemblyName Major.Minor.Revision (PortableRest
        ///     Major.Minor.Revision)
        /// </remarks>
        void SetUserAgent<T>(string displayName = null);

        /// <summary>
        ///     Executes an asynchronous request to the given resource and deserializes the response to an object of T.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="restRequest">The RestRequest to execute.</param>
        /// <returns>An object of T.</returns>
        Task<T> ExecuteAsync<T>(RestRequest restRequest) where T : class;
    }
}