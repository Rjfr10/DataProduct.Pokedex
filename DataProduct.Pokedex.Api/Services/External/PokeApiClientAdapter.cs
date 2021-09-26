using PokeApiNet;

namespace DataProduct.Pokedex.Api.Services.External
{
    /// <summary>
    /// Used to allow for correct use of Dependancy injection in Startup and for ease of mocking in unit tests.
    /// </summary>
    public class PokeApiClientAdapter : PokeApiClient, IPokeApiClient
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public PokeApiClientAdapter() : base() { }
    }
}