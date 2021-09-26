namespace DataProduct.Pokedex.Api.Models.Configuration
{
    /// <summary>
    /// Custom variables for application setup.
    /// These values are added to IOptionsMonitor and can be updated during runtime of the application without a restart.
    /// </summary>
    public class VariableOptions
    {
        /// <summary>
        /// The name of the parent section in appsettings.
        /// </summary>
        public const string SectionName = "Variables";

        /// <summary>
        /// A toggle to turn on/off the translation request to external provider.
        /// </summary>
        public bool TranslateEnabled { get; set; }

        /// <summary>
        /// Habitats that should be translated to the Yoda transation.
        /// </summary>
        public string[] HabitatsToTranslateToYoda { get; set; }

        /// <summary>
        /// A toggle to turn on/off the translation to Yoda on legendary status.
        /// </summary>
        public bool LegendaryTranslateToYoda { get; set; }
        
        /// <summary>
        /// The language used when retrieving the description to respond with for the supplied pokemon.
        /// </summary>
        public string FlavorTextLanguage { get; set; }
    }
}