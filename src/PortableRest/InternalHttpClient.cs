using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace PortableRest
{
    /// <summary>
    ///     This interface is for testing purposes only
    /// </summary>
    internal interface IInternalHttpClient : IDisposable
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        Uri BaseAddress { get; set; }
        TimeSpan Timeout { get; set; }
        long MaxResponseContentBufferSize { get; set; }
        Task<string> GetStringAsync(string requestUri);
        Task<string> GetStringAsync(Uri requestUri);
        Task<byte[]> GetByteArrayAsync(string requestUri);
        Task<byte[]> GetByteArrayAsync(Uri requestUri);
        Task<Stream> GetStreamAsync(string requestUri);
        Task<Stream> GetStreamAsync(Uri requestUri);
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> GetAsync(Uri requestUri);
        Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption);
        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption);
        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken);
        Task<HttpResponseMessage> DeleteAsync(string requestUri);
        Task<HttpResponseMessage> DeleteAsync(Uri requestUri);
        Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken);
        Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken);
        void CancelPendingRequests();
    }

    /// <summary>
    ///     Wrapper of <see cref="System.Net.Http.HttpClient" />.
    /// </summary>
    internal class InternalHttpClient : HttpClient, IInternalHttpClient
    {
        public InternalHttpClient(HttpMessageHandler handler)
            : base(handler)
        { }

        public InternalHttpClient(HttpMessageHandler handler, bool disposeHandler)
            : base(handler, disposeHandler)
        { }
    }
}