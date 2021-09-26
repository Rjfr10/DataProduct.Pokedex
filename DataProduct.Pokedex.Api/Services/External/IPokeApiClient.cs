using PokeApiNet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Services.External
{
    /// <summary>
    /// Interface for External PokeApiClient library.
    /// </summary>
    public interface IPokeApiClient {
        /// <inheritdoc />
        void ClearCache();
        /// <inheritdoc />
        void ClearResourceCache();
        /// <inheritdoc />
        void ClearResourceCache<T>() where T : ResourceBase;
        /// <inheritdoc />
        void ClearResourceListCache<T>() where T : ResourceBase;
        /// <inheritdoc />
        void ClearResourceListCache();
        /// <inheritdoc />
        void Dispose();
        /// <inheritdoc />
        Task<ApiResourceList<T>> GetApiResourcePageAsync<T>(int limit, int offset, CancellationToken cancellationToken = default) where T : ApiResource;
        /// <inheritdoc />
        Task<ApiResourceList<T>> GetApiResourcePageAsync<T>(CancellationToken cancellationToken = default) where T : ApiResource;
        /// <inheritdoc />
        Task<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(CancellationToken cancellationToken = default) where T : NamedApiResource;
        /// <inheritdoc />
        Task<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(int limit, int offset, CancellationToken cancellationToken = default) where T : NamedApiResource;
        /// <inheritdoc />
        Task<T> GetResourceAsync<T>(UrlNavigation<T> urlResource) where T : ResourceBase;
        /// <inheritdoc />
        Task<T> GetResourceAsync<T>(UrlNavigation<T> urlResource, CancellationToken cancellationToken) where T : ResourceBase;
        /// <inheritdoc />
        Task<T> GetResourceAsync<T>(string name, CancellationToken cancellationToken) where T : NamedApiResource;
        /// <inheritdoc />
        Task<T> GetResourceAsync<T>(string name) where T : NamedApiResource;
        /// <inheritdoc />
        Task<T> GetResourceAsync<T>(int id, CancellationToken cancellationToken) where T : ResourceBase;
        /// <inheritdoc />
        Task<T> GetResourceAsync<T>(int id) where T : ResourceBase;
        /// <inheritdoc />
        Task<List<T>> GetResourceAsync<T>(IEnumerable<UrlNavigation<T>> collection, CancellationToken cancellationToken) where T : ResourceBase;
        /// <inheritdoc />
        Task<List<T>> GetResourceAsync<T>(IEnumerable<UrlNavigation<T>> collection) where T : ResourceBase;
    }
}