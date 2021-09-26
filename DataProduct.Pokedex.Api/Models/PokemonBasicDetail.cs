using DataProduct.Pokedex.Api.Models.Enums;
using System.Text.Json.Serialization;

namespace DataProduct.Pokedex.Api.Models
{
    /// <summary>
    /// Basic Pokemon Response Object.
    /// </summary>
    public class PokemonBasicDetail
    {
        /// <summary>
        /// Pokemon's name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Pokemon's standard description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Pokemons's habitat.
        /// </summary>
        public string Habitat { get; set; }

        /// <summary>
        /// Pokemon's is_legendary status
        /// </summary>
        public bool IsLegendary { get; set; }

        /// <summary>
        /// Response to determine if the value has been translated or not
        /// </summary>
        public bool IsTranslated { get; set; } = false;

        /// <summary>
        /// What type of translation was used.
        /// </summary>
        public TranslationTypeEnum TranslationTypeId { get; set; } = TranslationTypeEnum.Standard;

        /// <summary>
        /// The name of the translation used of TranslationTypeId.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TranslationTypeEnum TranslationType
        {
            get { return TranslationTypeId; }
            private set
            {
                TranslationType = TranslationTypeId;
            }
        }
    }
}