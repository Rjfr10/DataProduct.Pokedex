using DataProduct.Pokedex.Api.Controllers;
using DataProduct.Pokedex.Api.Models.External;
using DataProduct.Pokedex.Api.Helpers;
using DataProduct.Pokedex.Api.Models;
using DataProduct.Pokedex.Api.Models.Configuration;
using DataProduct.Pokedex.Api.Models.Enums;
using DataProduct.Pokedex.Api.Services;
using DataProduct.Pokedex.Api.Services.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json;
using System.Threading;
using System.Net;

namespace DataProduct.Pokedex.Api.Test.Controllers
{
    public class PokemonControllerTests
    {
        private Mock<IConfigurationRoot> _mockConfiguration;
        private Mock<IOptionsMonitor<VariableOptions>> _mockVariableOptions;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<IPokeApiClient> _mockPokeApi;
        private Mock<ILogger<TranslationService>> _mockTranslationServiceLogger;
        private Mock<ILogger<PokemonService>> _mockPokemonServiceLogger;

        List<NamedApiResource<PokemonSpecies>> AvailablePokemon = new List<NamedApiResource<PokemonSpecies>>
        {
            new NamedApiResource<PokemonSpecies>() { Name="Charizard",  Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Dragonite",  Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Ninetales",  Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Pikachu",    Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Zaptos",     Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Suicune",    Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Arcanine",   Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Snorlax",    Url="" },
            new NamedApiResource<PokemonSpecies>() { Name="Psyduck",    Url="" },
        };
        List<PokemonSpecies> AvailablePokemonSpecies = new List<PokemonSpecies>
        {
            new PokemonSpecies() { Name = "1", IsLegendary = true,
                Habitat = new NamedApiResource<PokemonHabitat>() { Name = "cave" } ,
                FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>() { new PokemonSpeciesFlavorTexts() { FlavorText = "1 - Should Yoda Translate", Language = new NamedApiResource<Language>(){ Name = "en" } } }
            },
            new PokemonSpecies() { Name = "2", IsLegendary = false,
                Habitat = new NamedApiResource<PokemonHabitat>() { Name = "cave" } ,
                FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>() { new PokemonSpeciesFlavorTexts() { FlavorText = "2 - Should Yoda Translate", Language = new NamedApiResource<Language>(){ Name = "en" } } }
            },
            new PokemonSpecies() { Name = "3", IsLegendary = true,
                Habitat = new NamedApiResource<PokemonHabitat>() { Name = "mountain" } ,
                FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>() { new PokemonSpeciesFlavorTexts() { FlavorText = "3 - Should Yoda Translate", Language = new NamedApiResource<Language>(){ Name = "en" } } }
            },
            new PokemonSpecies() { Name = "4", IsLegendary = false,
                Habitat = new NamedApiResource<PokemonHabitat>() { Name = "ocean" } ,
                FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>() { new PokemonSpeciesFlavorTexts() { FlavorText = "4 - Should Shakespeare Translate", Language = new NamedApiResource<Language>(){ Name = "en" } } }
            },
            new PokemonSpecies() { Name = "5", IsLegendary = false,
                Habitat = new NamedApiResource<PokemonHabitat>() { Name = "tunnel" } ,
                FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>() { new PokemonSpeciesFlavorTexts() { FlavorText = "5 - Should Shakespeare Translate", Language = new NamedApiResource<Language>(){ Name = "en" } } }
            },
            new PokemonSpecies() { Name = "6", IsLegendary = false,
                Habitat = new NamedApiResource<PokemonHabitat>() { Name = "forrest" } ,
                FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>() { new PokemonSpeciesFlavorTexts() { FlavorText = "6 - Should Shakespeare Translate", Language = new NamedApiResource<Language>(){ Name = "en" } } }
            }
        };

        public PokemonControllerTests()
        {
            _mockConfiguration = new Mock<IConfigurationRoot>();
            _mockVariableOptions = new Mock<IOptionsMonitor<VariableOptions>>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockPokeApi = new Mock<IPokeApiClient>();
            _mockPokemonServiceLogger = new Mock<ILogger<PokemonService>>();
            _mockTranslationServiceLogger = new Mock<ILogger<TranslationService>>();
        }

        private NamedApiResource<PokemonSpecies> GetPokemonNamedResourceFromList(string pokemonName)
        {
            return AvailablePokemon.FirstOrDefault(pokemon => string.Equals(pokemon.Name, pokemonName, StringComparison.OrdinalIgnoreCase));
        }
        
        private PokemonSpecies GetPokemonSpeciesFromList(string nameAsNo)
        {
            return AvailablePokemonSpecies.FirstOrDefault(pokemon => string.Equals(pokemon.Name, nameAsNo, StringComparison.OrdinalIgnoreCase));
        }

        private void ConfigurationSettings()
        {
            _mockConfiguration.SetupGet(x => x.GetSection("ExternalSource:FunTranslations").Value)
                              .Returns("https://api.funtranslations.com/translate");
        }

        private void ConfigureOptions(VariableOptions options)
        {
            _mockVariableOptions.SetupGet(x => x.CurrentValue)
                                .Returns(options);
        }

        private void ConfigurePokeApi(string pokemonName, NamedApiResource<PokemonSpecies> namedResourcePokemonSpecies, PokemonSpecies pokemonSpecies)
        {
            // How the client behaves.
            if (namedResourcePokemonSpecies is null 
             || namedResourcePokemonSpecies.Name is null)
            {
                _mockPokeApi.Setup(x => x.GetResourceAsync<Pokemon>(pokemonName)).ThrowsAsync(new KeyNotFoundException("Pokemon does not exist."));
                return;
            }
            if (pokemonSpecies is null
             || pokemonSpecies.Name is null)
            { 
                _mockPokeApi.Setup(x => x.GetResourceAsync(It.IsAny<UrlNavigation<PokemonSpecies>>())).ThrowsAsync(new KeyNotFoundException("Pokemon does not exist."));
            }
            _mockPokeApi.Setup(x => x.GetResourceAsync<Pokemon>(pokemonName))
                        .ReturnsAsync(new Pokemon() { Species = namedResourcePokemonSpecies });
            
            _mockPokeApi.Setup(x => x.GetResourceAsync(It.IsAny<UrlNavigation<PokemonSpecies>>()))
                        .ReturnsAsync(pokemonSpecies);
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

        private PokemonController CompleteSetup(
            Mock<IConfigurationRoot> mockConfiguration,
            Mock<IOptionsMonitor<VariableOptions>> mockVariableOptions,
            Mock<ILogger<PokemonService>> mockPokemonServiceLogger,
            Mock<ILogger<TranslationService>> mockTranslationServiceLogger,
            Mock<IHttpClientFactory> mockHttpClientFactory, 
            Mock<IPokeApiClient> mockPokeApi
            )
        {
            var httpClientHelper = new HttpClientHelper(mockHttpClientFactory.Object);
            var translationService = new TranslationService(mockTranslationServiceLogger.Object, mockVariableOptions.Object, mockConfiguration.Object, httpClientHelper);
            var pokemonService = new PokemonService(mockPokemonServiceLogger.Object, mockVariableOptions.Object, mockPokeApi.Object, translationService);
            return new PokemonController(pokemonService);
        }
        
        [Theory]
        [InlineData("Charizard", false, "1", "en","cave",  true, true)]
        [InlineData("Dragonite", false, "2", "en", "cave", true, true)]
        [InlineData("Ninetales", false, "3", "en", "cave", true, true)]
        [InlineData("Pikachu",   false, "4", "en", "cave", true, true)]
        public async Task Test_GivenValidPokemonName_ShouldReturnBasicInformationWithStandardTranslation(
            string pokemonName, bool translate, string nameAsNo, string flavorTextLanguage, string habitatToTranslate, bool translateEnabled, bool translateOnLegendary
            )
        {
            // Setup
            var pokemonNamedResource = GetPokemonNamedResourceFromList(pokemonName);
            var pokemonSpecies = GetPokemonSpeciesFromList(nameAsNo);
            var description = pokemonSpecies.FlavorTextEntries.FirstOrDefault().FlavorText;

            ConfigurationSettings();
            ConfigureOptions(new VariableOptions()
            {
                FlavorTextLanguage = flavorTextLanguage,
                HabitatsToTranslateToYoda = new string[] { habitatToTranslate },
                TranslateEnabled = translateEnabled,
                LegendaryTranslateToYoda = translateOnLegendary
            });
            ConfigurePokeApi(pokemonName, pokemonNamedResource, pokemonSpecies);
            
            var controller = CompleteSetup(_mockConfiguration, _mockVariableOptions, _mockPokemonServiceLogger, _mockTranslationServiceLogger, _mockHttpClientFactory, _mockPokeApi);

            // Act
            ActionResult<PokemonBasicDetail> result = (translate) ? await controller.GetTranslated(pokemonName) : await controller.GetBasic(pokemonName);

            // Assert
            var apiResult = Assert.IsType<PokemonBasicDetail>(result.Value);
            Assert.Equal(pokemonName, apiResult.Name);
            Assert.NotEmpty(apiResult.Description);
            Assert.Equal(description, apiResult.Description);
            Assert.NotEmpty(apiResult.Habitat);
            Assert.Equal(pokemonSpecies.Habitat.Name, apiResult.Habitat);
            Assert.Equal(TranslationTypeEnum.Standard, apiResult.TranslationTypeId);
            Assert.Equal(TranslationTypeEnum.Standard, apiResult.TranslationType);
            Assert.False(apiResult.IsTranslated);
        }

        [Theory]
        [InlineData("Charizard", true, "1", "en", "cave",     true, true )]
        [InlineData("Dragonite", true, "2", "en", "cave",     true, false)]
        [InlineData("Ninetales", true, "3", "en", "mountain", true, true )]
        public async Task Test_GivenValidPokemonName_ShouldReturnTranslatedInformationForYoda(
            string pokemonName, bool translate, string nameAsNo, string flavorTextLanguage, string habitatToTranslate, bool translateEnabled, bool translateOnLegendary
            )
        {
            // Setup
            var pokemonNamedResource = GetPokemonNamedResourceFromList(pokemonName);
            var pokemonSpecies = GetPokemonSpeciesFromList(nameAsNo);
            var description = pokemonSpecies.FlavorTextEntries.FirstOrDefault().FlavorText;

            ConfigurationSettings();
            ConfigureOptions(new VariableOptions()
            {
                FlavorTextLanguage = flavorTextLanguage,
                HabitatsToTranslateToYoda = new string[] { habitatToTranslate },
                TranslateEnabled = translateEnabled,
                LegendaryTranslateToYoda = translateOnLegendary
            });
            ConfigurePokeApi(pokemonName, pokemonNamedResource, pokemonSpecies);

            var httpResponse = new FunTranslationsResponse()
            {
                Success = new Models.External.Success() { Total = 1 },
                Contents = new Models.External.Contents()
                {
                    Translated = "Should Yoda Translate",
                    Text = description,
                    Translation = "Yoda"
                }
            };
            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(httpResponse))
            };
            _mockHttpClientFactory = ConfigureHttpClientFactory(httpResponseMessage);

            var controller = CompleteSetup(_mockConfiguration, _mockVariableOptions, _mockPokemonServiceLogger, _mockTranslationServiceLogger, _mockHttpClientFactory, _mockPokeApi);

            // Act
            ActionResult<PokemonBasicDetail> result = (translate) ? await controller.GetTranslated(pokemonName) : await controller.GetBasic(pokemonName);

            // Assert
            var apiResult = Assert.IsType<PokemonBasicDetail>(result.Value);
            Assert.Equal(pokemonName, apiResult.Name);
            Assert.NotEmpty(apiResult.Description);
            Assert.Contains(apiResult.Description, description);
            Assert.NotEmpty(apiResult.Habitat);
            Assert.Equal(pokemonSpecies.Habitat.Name, apiResult.Habitat);
            Assert.Equal(TranslationTypeEnum.Yoda, apiResult.TranslationTypeId);
            Assert.Equal(TranslationTypeEnum.Yoda, apiResult.TranslationType);
            Assert.True(apiResult.IsTranslated);
        }

        [Theory]
        [InlineData("Pikachu", true, "4", "en", "cave", true, false)]
        [InlineData("Zaptos",  true, "5", "en", "cave", true, false)]
        [InlineData("Suicune", true, "6", "en", "cave", true, false)]
        public async Task Test_GivenValidPokemonName_ShouldReturnTranslatedInformationForShakespeare(
            string pokemonName, bool translate, string nameAsNo, string flavorTextLanguage, string habitatToTranslate, bool translateEnabled, bool translateOnLegendary
            )
        {
            // Setup
            var pokemonNamedResource = GetPokemonNamedResourceFromList(pokemonName);
            var pokemonSpecies = GetPokemonSpeciesFromList(nameAsNo);
            var description = pokemonSpecies.FlavorTextEntries.FirstOrDefault().FlavorText;

            ConfigurationSettings();
            ConfigureOptions(new VariableOptions()
            {
                FlavorTextLanguage = flavorTextLanguage,
                HabitatsToTranslateToYoda = new string[] { habitatToTranslate },
                TranslateEnabled = translateEnabled,
                LegendaryTranslateToYoda = translateOnLegendary
            });
            ConfigurePokeApi(pokemonName, pokemonNamedResource, pokemonSpecies);

            var httpResponse = new FunTranslationsResponse()
            {
                Success = new Models.External.Success() { Total = 1 },
                Contents = new Models.External.Contents()
                {
                    Translated = "Should Shakespeare Translate",
                    Text = description,
                    Translation = "Shakespeare"
                }
            };
            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(httpResponse))
            };
            _mockHttpClientFactory = ConfigureHttpClientFactory(httpResponseMessage);

            var controller = CompleteSetup(_mockConfiguration, _mockVariableOptions, _mockPokemonServiceLogger, _mockTranslationServiceLogger, _mockHttpClientFactory, _mockPokeApi);

            // Act
            ActionResult<PokemonBasicDetail> result = (translate) ? await controller.GetTranslated(pokemonName) : await controller.GetBasic(pokemonName);

            // Assert
            var apiResult = Assert.IsType<PokemonBasicDetail>(result.Value);
            Assert.Equal(pokemonName, apiResult.Name);
            Assert.NotEmpty(apiResult.Description);
            Assert.Contains(apiResult.Description, description);
            Assert.NotEmpty(apiResult.Habitat);
            Assert.Equal(pokemonSpecies.Habitat.Name, apiResult.Habitat);
            Assert.Equal(TranslationTypeEnum.Shakespeare, apiResult.TranslationTypeId);
            Assert.Equal(TranslationTypeEnum.Shakespeare, apiResult.TranslationType);
            Assert.True(apiResult.IsTranslated);
        }

        [Theory]
        [InlineData("1111111",  true,  "1", "en", "cave", true, false)]
        [InlineData("aaaaaaa",  false, "1", "en", "cave", true, false)]
        [InlineData("fffffff",  true,  "6", "en", "cave", true, false)]
        [InlineData("abcdef",   false, "6", "en", "cave", true, false)]
        public async Task Test_GivenInValidPokemonName_ShouldReturnFailure(
            string pokemonName, bool translate, string nameAsNo, string flavorTextLanguage, string habitatToTranslate, bool translateEnabled, bool translateOnLegendary
            )
        {
            // Setup
            var pokemonNamedResource = GetPokemonNamedResourceFromList(pokemonName);
            var pokemonSpecies = GetPokemonSpeciesFromList(nameAsNo);
            
            ConfigurationSettings();
            ConfigureOptions(new VariableOptions()
            {
                FlavorTextLanguage = flavorTextLanguage,
                HabitatsToTranslateToYoda = new string[] { habitatToTranslate },
                TranslateEnabled = translateEnabled,
                LegendaryTranslateToYoda = translateOnLegendary
            });
            ConfigurePokeApi(pokemonName, pokemonNamedResource, pokemonSpecies);

            var controller = CompleteSetup(_mockConfiguration, _mockVariableOptions, _mockPokemonServiceLogger, _mockTranslationServiceLogger, _mockHttpClientFactory, _mockPokeApi);

            // Act
            try
            {
                ActionResult<PokemonBasicDetail> result = (translate) ? await controller.GetTranslated(pokemonName) : await controller.GetBasic(pokemonName);
            }
            catch(Exception ex)
            {
                Assert.Equal("Pokemon does not exist.", ex.Message);
            }
        }
    }
}