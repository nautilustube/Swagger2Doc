using System.Net;

namespace Swagger2Doc.Libs
{
    /// <summary>
    /// Static HttpClient With lazy 初始化
    /// </summary>
    public class HttpInstance
    {
        public static HttpClient Instance { get { return lazy.Value; } }
        private static readonly Lazy<HttpClient> lazy = new Lazy<HttpClient>(
            () =>
            {
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    }
                };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.Timeout = TimeSpan.FromSeconds(50 * 60);
                return httpClient;
            });

    }
}
