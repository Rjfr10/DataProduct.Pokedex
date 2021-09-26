using DataProduct.Pokedex.Api.Models;
using DataProduct.Pokedex.Api.Models.Configuration;
using DataProduct.Pokedex.Api.Services.External;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeApiNet;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Services
{
    /// <summary>
    /// Pokemon Service Class for information retrieval.
    /// </summary>
    public class PokemonService : IPokemonService
    {
        
        private readonly IPokeApiClient _pokeApiclient;
        private readonly ITranslationService _translationService;
        private readonly ILogger<PokemonService> _logger;
        private readonly IOptionsMonitor<VariableOptions> _optionsDelegate;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="optionsDelegate"></param>
        /// <param name="pokeApiclient"></param>
        /// <param name="translationService"></param>
        public PokemonService (
            ILogger<PokemonService> logger,
            IOptionsMonitor<VariableOptions> optionsDelegate,
            IPokeApiClient pokeApiclient,
            ITranslationService translationService
            )
        {
            _logger = logger;
            _optionsDelegate = optionsDelegate;
            _pokeApiclient = pokeApiclient;
            _translationService = translationService;
        }

        /// <summary>
        /// Provides a basic overview of the pokemon supplied including their habitat and if they are of legendary status.
        /// </summary>
        /// <param name="pokemonName">The given name of a pokemon.</param>
        /// <param name="translate">Determines if the request requires a translated description.</param>
        /// <returns></returns>
        public async Task<PokemonBasicDetail> GetInformation(string pokemonName, bool translate = false)
        {
            Pokemon pokemon = await _pokeApiclient.GetResourceAsync<Pokemon>(pokemonName);
            PokemonSpecies species = await _pokeApiclient.GetResourceAsync(pokemon.Species);
            PokemonSpeciesFlavorTexts flavorTexts = species.FlavorTextEntries.FirstOrDefault(flavor => string.Equals(flavor.Language.Name, _optionsDelegate.CurrentValue.FlavorTextLanguage, StringComparison.OrdinalIgnoreCase));
            var basicDetail = new PokemonBasicDetail()
            {
                Name = pokemonName,
                Description = string.IsNullOrEmpty(flavorTexts.FlavorText) ? "" : flavorTexts.FlavorText,
                Habitat = species.Habitat.Name,
                IsLegendary = species.IsLegendary
            };
            if (string.IsNullOrEmpty(basicDetail.Description))
            {
                _logger.LogWarning($"Failure attempting to retrieve a description for the pokemon: {pokemonName}");
                return basicDetail;
            }
            if (translate && _optionsDelegate.CurrentValue.TranslateEnabled)
            {
                return await _translationService.TranslateResponse(basicDetail);
            }
            return basicDetail;
        }
    }
}
