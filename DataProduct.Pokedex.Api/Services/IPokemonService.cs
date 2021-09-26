using DataProduct.Pokedex.Api.Models;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Services
{
    /// <summary>
    /// Pokemon Interface for information retrieval
    /// </summary>
    public interface IPokemonService
    {
        /// <inheritdoc />
        Task<PokemonBasicDetail> GetInformation(string pokemonName, bool translate = false);
    }
}