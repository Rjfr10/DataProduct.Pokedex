using System;
using System.ComponentModel;
using System.Linq;

namespace DataProduct.Pokedex.Api.Extensions
{
    /// <summary>
    /// Extension Functionality for Enums
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Attempts to retrieve the description attribute assigned to the Enum type.
        /// Used to get the translation route.
        /// </summary>
        /// <param name="value">The value of the Enum.</param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            DescriptionAttribute descriptionAttribute = value.GetType()
                                                             .GetField(value.ToString())
                                                             .GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                             .SingleOrDefault() as DescriptionAttribute;
            if (descriptionAttribute is null)
            {
                throw new ArgumentException("No Description Set on Enum value!");
            }
            return descriptionAttribute.Description;
        }
    }
}
