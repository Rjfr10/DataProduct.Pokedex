using DataProduct.Pokedex.Api.Helpers;
using DataProduct.Pokedex.Api.Models;
using DataProduct.Pokedex.Api.Models.Configuration;
using DataProduct.Pokedex.Api.Models.Enums;
using DataProduct.Pokedex.Api.Models.External;
using DataProduct.Pokedex.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DataProduct.Pokedex.Api.Test.Services
{
    public class TranslationServiceTests
    {
        private Mock<IConfigurationRoot> _mockConfiguration;
        private Mock<IOptionsMonitor<VariableOptions>> _mockVariableOptions;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<ILogger<TranslationService>> _mockTranslationServiceLogger;

        public TranslationServiceTests()
        {
            _mockConfiguration = new Mock<IConfigurationRoot>();
            _mockVariableOptions = new Mock<IOptionsMonitor<VariableOptions>>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockTranslationServiceLogger = new Mock<ILogger<TranslationService>>();
        }

        private Mock<IHttpClientFactory> ConfigureHttpClientFactory(HttpResponseMessage httpResponseMessage)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                                  .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                                  .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://mock"),
            };
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>()))
                                  .Returns(httpClient);

            return _mockHttpClientFactory;
        }

        [Fact]
        public async Task Test_GivenPokemonIsLegendary_ShouldReturnYoda()
        {
            var pokemonDetails = new PokemonBasicDetail()
            {
                Name = "Charizard",
                Description = "This is the description",
                Habitat = "NotSpecified",
                IsLegendary = true
            };
            var httpResponse = new FunTranslationsResponse()
            {
                Success = new Models.External.Success() { Total = 1 },
                Contents = new Models.External.Contents()
                {
                    Translated = "",
                    Text = "The End Goal",
                    Translation = ""
                }
            };
            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(httpResponse))
            };
            var options = new VariableOptions()
            {
                FlavorTextLanguage = "en",
                HabitatsToTranslateToYoda = new string[] { "cave" },
                TranslateEnabled = true,
                LegendaryTranslateToYoda = true
            };

            _mockVariableOptions.SetupGet(x => x.CurrentValue)
                                .Returns(options);

            _mockConfiguration.SetupGet(x => x.GetSection("ExternalSource:FunTranslations").Value)
                              .Returns("https://api.funtranslations.com/translate");

            var mockHttpClientFactory = ConfigureHttpClientFactory(httpResponseMessage);

            var httpClientHelper = new HttpClientHelper(mockHttpClientFactory.Object);
            var translationService = new TranslationService(_mockTranslationServiceLogger.Object, _mockVariableOptions.Object, _mockConfiguration.Object, httpClientHelper);

            var result = await translationService.TranslateResponse(pokemonDetails);
            Assert.Equal(pokemonDetails.Name,result.Name);
            Assert.NotEqual("cave", pokemonDetails.Habitat);
            Assert.True(result.IsLegendary);
            Assert.True(result.IsTranslated);
            Assert.Equal(TranslationTypeEnum.Yoda, result.TranslationTypeId);
            Assert.Equal(TranslationTypeEnum.Yoda, result.TranslationType);
        }

        [Fact]
        public async Task Test_GivenPokemonHabitIsCave_ShouldReturnYoda()
        {
            var pokemonDetails = new PokemonBasicDetail()
            {
                Name = "Charizard",
                Description = "This is the description",
                Habitat = "cave",
                IsLegendary = false
            };
            var httpResponse = new FunTranslationsResponse()
            {
                Success = new Models.External.Success() { Total = 1 },
                Contents = new Models.External.Contents()
                {
                    Translated = "",
                    Text = "The End Goal",
                    Translation = ""
                }
            };
            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(httpResponse))
            };
            var options = new VariableOptions()
            {
                FlavorTextLanguage = "en",
                HabitatsToTranslateToYoda = new string[] { "cave" },
                TranslateEnabled = true,
                LegendaryTranslateToYoda = true
            };

            _mockVariableOptions.SetupGet(x => x.CurrentValue)
                                .Returns(options);

            _mockConfiguration.SetupGet(x => x.GetSection("ExternalSource:FunTranslations").Value)
                              .Returns("https://api.funtranslations.com/translate");

            var mockHttpClientFactory = ConfigureHttpClientFactory(httpResponseMessage);

            var httpClientHelper = new HttpClientHelper(mockHttpClientFactory.Object);
            var translationService = new TranslationService(_mockTranslationServiceLogger.Object, _mockVariableOptions.Object, _mockConfiguration.Object, httpClientHelper);

            var result = await translationService.TranslateResponse(pokemonDetails);
            Assert.Equal(pokemonDetails.Name, result.Name);
            Assert.False(result.IsLegendary);
            Assert.Equal("cave", pokemonDetails.Habitat);
            Assert.True(result.IsTranslated);
            Assert.Equal(TranslationTypeEnum.Yoda, result.TranslationTypeId);
            Assert.Equal(TranslationTypeEnum.Yoda, result.TranslationType);
        }

        [Fact]
        public async Task Test_GivenPokemonHabitIsNotCaveAnPokemonIsNotLegendary_ShouldReturnShakespeare()
        {
            var pokemonDetails = new PokemonBasicDetail()
            {
                Name = "Charizard",
                Description = "This is the description",
                Habitat = "NotSpecified",
                IsLegendary = false
            };
            var httpResponse = new FunTranslationsResponse()
            {
                Success = new Models.External.Success() { Total = 1 },
                Contents = new Models.External.Contents()
                {
                    Translated = "",
                    Text = "The End Goal",
                    Translation = ""
                }
            };
            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(httpResponse))
            };
            var options = new VariableOptions()
            {
                FlavorTextLanguage = "en",
                HabitatsToTranslateToYoda = new string[] { "cave" },
                TranslateEnabled = true,
                LegendaryTranslateToYoda = true
            };

            _mockVariableOptions.SetupGet(x => x.CurrentValue)
                                .Returns(options);

            _mockConfiguration.SetupGet(x => x.GetSection("ExternalSource:FunTranslations").Value)
                              .Returns("https://api.funtranslations.com/translate");

            var mockHttpClientFactory = ConfigureHttpClientFactory(httpResponseMessage);

            var httpClientHelper = new HttpClientHelper(mockHttpClientFactory.Object);
            var translationService = new TranslationService(_mockTranslationServiceLogger.Object, _mockVariableOptions.Object, _mockConfiguration.Object, httpClientHelper);

            var result = await translationService.TranslateResponse(pokemonDetails);
            Assert.Equal(pokemonDetails.Name, result.Name);
            Assert.NotEqual("cave", pokemonDetails.Habitat);
            Assert.False(result.IsLegendary);
            Assert.True(result.IsTranslated);
            Assert.Equal(TranslationTypeEnum.Shakespeare, result.TranslationTypeId);
            Assert.Equal(TranslationTypeEnum.Shakespeare, result.TranslationType);
        }
    }
}
