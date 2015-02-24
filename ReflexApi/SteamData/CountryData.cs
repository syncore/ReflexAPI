// ReSharper disable InconsistentNaming
namespace ReflexAPI.SteamData
{
    /// <summary>
    /// Model class that represents a server's country information.
    /// </summary>
    /// <remarks>
    /// Ignore ReSharper warnings about capitalization since JSON responses
    /// are not typically capitalized in APIs.
    /// </remarks>
    public class CountryData
    {
        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        /// <value>
        /// The name of the country.
        /// </value>
        public string countryName { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string countryCode { get; set; }
    }
}