using DataProduct.Pokedex.Api.Models;
using DataProduct.Pokedex.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Controllers
{
    /// <summary>
    /// The Controller responsible for all pokemon related calls.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        /// <summary>
        /// Pokemon Controller Ctor
        /// </summary>
        /// <param name="pokemonService">An instance of the pokemon service</param>
        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        /// <summary>
        /// Endpoint 1 - Basic Pokemon Information
        /// </summary>
        /// <param name="pokemonName">The given name of a pokemon.</param>
        /// <returns></returns>
        [HttpGet("{pokemonName}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Endpoint 1 - Basic Pokemon Information", typeof(PokemonBasicDetail))]
        public async Task<ActionResult<PokemonBasicDetail>> GetBasic(string pokemonName)
        {
            return await _pokemonService.GetInformation(pokemonName);
        }

        /// <summary>
        /// Endpoint 2 - Translated Pokemon Information
        /// </summary>
        /// <param name="pokemonName">The given name of a pokemon.</param>
        /// <returns></returns>
        [HttpGet("translated/{pokemonName}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Endpoint 2 - Translated Pokemon Information", typeof(PokemonBasicDetail))]
        public async Task<ActionResult<PokemonBasicDetail>> GetTranslated(string pokemonName)
        {
            return await _pokemonService.GetInformation(pokemonName, true);
        }
    }
}