using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Helpers
{
    /// <summary>
    /// Helper Class for Http Requests
    /// This currently is limited to POST only since it is the only call used.
    /// </summary>
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Http Client Helper Ctor.
        /// </summary>
        /// <param name="httpClientFactory">An instance of the http client factory setup in DI.</param>
        public HttpClientHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Sends an HTTP POST request amd returns the data received in the content of the response.
        /// </summary>
        /// <typeparam name="T">The expected type to receive.</typeparam>
        /// <param name="message">An HttpRequestMessage.</param>
        /// <returns></returns>
        public async Task<T> PerformPostRequest<T>(string httpClientName, HttpRequestMessage message)
        {
            T data = default;
            var httpClient = _httpClientFactory.CreateClient(httpClientName);
            using (var response = await httpClient.SendAsync(message))
            {
                response.EnsureSuccessStatusCode();
                if (response.Content is object)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    data = await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
            }
            return data;
        }
    }
}
