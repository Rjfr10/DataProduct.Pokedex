using DataProduct.Pokedex.Api.Models;
using DataProduct.Pokedex.Api.Models.Enums;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Services
{
    /// <summary>
    /// Translation Service Interface.
    /// </summary>
    public interface ITranslationService
    {
        /// <inheritdoc />
        Task<PokemonBasicDetail> TranslateResponse(PokemonBasicDetail basicDetail);

        /// <inheritdoc />
        Task<PokemonBasicDetail> PerformTranslation(PokemonBasicDetail basicDetail, TranslationTypeEnum translationType);
    }
}
