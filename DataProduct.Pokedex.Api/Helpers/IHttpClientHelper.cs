using System.Net.Http;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Helpers
{
    /// <summary>
    /// Helper Interace for Http Requests
    /// </summary>
    public interface IHttpClientHelper
    {
        /// <inheritdoc />
        Task<T> PerformPostRequest<T>(string httpClientName, HttpRequestMessage message);
    }
}