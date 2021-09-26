using System.ComponentModel;

namespace DataProduct.Pokedex.Api.Models.Enums
{
    /// <summary>
    /// Used to determine translation type applied to the response.
    /// The description is setup for routing the application calls to the external provider. cref=""
    /// </summary>
    public enum TranslationTypeEnum
    {
        Standard = 0,
        [Description("/yoda.json")]
        Yoda = 1,
        [Description("/shakespeare.json")]
        Shakespeare = 2
    }
}