using DataProduct.Pokedex.Api.Extensions;
using DataProduct.Pokedex.Api.Helpers;
using DataProduct.Pokedex.Api.Models;
using DataProduct.Pokedex.Api.Models.Configuration;
using DataProduct.Pokedex.Api.Models.Enums;
using DataProduct.Pokedex.Api.Models.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataProduct.Pokedex.Api.Services
{
    /// <summary>
    /// Translation Service Class.
    /// </summary>
    public class TranslationService : ITranslationService
    {
        private readonly ILogger<TranslationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOptionsMonitor<VariableOptions> _optionsDelegate;
        private readonly IHttpClientHelper _httpClient;

        /// <summary>
        /// Translation Service Ctor.
        /// </summary>
        /// <param name="logger">An instance of the logger.</param>
        /// <param name="optionsDelegate">An instance of IOptionsMonitor</param>
        /// <param name="configuration">An instance of IConfiguration</param>
        /// <param name="httpClient">An instance of IHttpClientFactory</param>
        public TranslationService(
            ILogger<TranslationService> logger,
            IOptionsMonitor<VariableOptions> optionsDelegate,
            IConfiguration configuration,
            IHttpClientHelper httpClient)
        {
            _logger = logger;
            _optionsDelegate = optionsDelegate;
            _configuration = configuration; 
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves the configuration setup and determines the translation to apply.
        /// </summary>
        /// <param name="basicDetail">The pokemon details object.</param>
        /// <returns></returns>
        public async Task<PokemonBasicDetail> TranslateResponse(PokemonBasicDetail basicDetail)
        {
            string[] habitats = _optionsDelegate.CurrentValue.HabitatsToTranslateToYoda;
            var translateIfLegendary = Convert.ToBoolean(_optionsDelegate.CurrentValue.LegendaryTranslateToYoda);
            if ((translateIfLegendary && basicDetail.IsLegendary)
             || habitats.Contains(basicDetail.Habitat))
            {
                return await PerformTranslation(basicDetail, TranslationTypeEnum.Yoda);
            }
            return await PerformTranslation(basicDetail, TranslationTypeEnum.Shakespeare);
        }

        /// <summary>
        /// Perform the translation of the pokemon details from basic to fun.
        /// </summary>
        /// <param name="basicDetail">The pokemon details object.</param>
        /// <param name="translationType">The type of translation to apply.</param>
        /// <returns></returns>
        public async Task<PokemonBasicDetail> PerformTranslation(PokemonBasicDetail basicDetail, TranslationTypeEnum translationType)
        {
            try
            {
                var endpoint = _configuration["ExternalSource:FunTranslations"];
                var route = translationType.GetEnumDescription();
                var requestBody = new StringContent(JsonSerializer.Serialize(new { text = basicDetail.Description }), Encoding.UTF8, "application/json");
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}{route}")
                {
                    Content = requestBody
                };
                var externalResponse = await _httpClient.PerformPostRequest<FunTranslationsResponse>("funtranslations", requestMessage);
                if (externalResponse.Contents is not null)
                {
                    basicDetail.Description = externalResponse.Contents.Translated;
                    basicDetail.IsTranslated = true;
                    basicDetail.TranslationTypeId = translationType;
                    return basicDetail;
                }
                throw new Exception($"Failed to receive a response from provider for pokemon: {basicDetail.Name} | Endpoint: {endpoint}{route} | Description {basicDetail.Description}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Translation request failed for pokemon: {basicDetail.Name} | Description: {basicDetail.Description} | Type: {translationType} | Message: {ex.Message}");
            }
            //Attempted to translate the description and a failure occured so will return the original detail and log the issue.
            return basicDetail;
        }
    }
}
