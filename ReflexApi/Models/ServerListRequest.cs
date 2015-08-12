namespace ReflexAPI.Models
{
    using System.Collections.Generic;
    using Enum;
    using ServiceStack;

    /// <summary>
    /// Class responsible for accepting the user's request (specified in URL) for the Reflex server
    /// list. <remarks> Note, the Route attribute specifies the part of the URL that this request is
    /// responsible for. It can be defined at the class level, as done here, or in a container in
    /// the ReflexApiHost.cs file. </remarks>
    /// </summary>
    [Route("/servers", "GET")]
    public class ServerListRequest : IReturn<ServerListResponse>
    {
        // All servers: http://the.webserver.com/servers
        // Filters (case insensitive): http://the.webserver.com/servers?filter1=type1&filter2=type2&filter3=type3...filterN=typeN

        /// <summary>
        /// Gets or sets the continent filter.
        /// </summary>
        /// <value>
        /// The continent filter.
        /// </value>
        public string Continent { get; set; }
        
        /// <summary>
        /// Gets or sets the country code filter.
        /// </summary>
        /// <value>The country code filter.</value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the gametype filter.
        /// </summary>
        /// <value>The gametype filter.</value>
        /// <remarks>Currently, Reflex uses Valve server keywords for the gametype.</remarks>
        public string GameType { get; set; }

        /// <summary>
        /// Gets or sets the has password filter.
        /// </summary>
        /// <value>The has password filter.</value>
        public bool? HasPassword { get; set; }

        /// <summary>
        /// Gets or sets the has players filter.
        /// </summary>
        /// <value>The has players filter.</value>
        public bool? HasPlayers { get; set; }

        /// <summary>
        /// Gets or sets the has vac protection filter.
        /// </summary>
        /// <value>The has vac protection filter.</value>
        public bool? HasVac { get; set; }

        /// <summary>
        /// Gets or sets the is not full filter.
        /// </summary>
        /// <value>The is not full filter.</value>
        public bool? IsNotFull { get; set; }

        /// <summary>
        /// Gets or sets the map filter.
        /// </summary>
        /// <value>The map filter.</value>
        public string Map { get; set; }

        /// <summary>
        /// Gets or sets the server operating system filter.
        /// </summary>
        /// <value>The server operating system filter.</value>
        public string Os { get; set; }

        /// <summary>
        /// Gets or sets the protocol filter.
        /// </summary>
        /// <value>The protocol filter.</value>
        public uint? Protocol { get; set; }

        /// <summary>
        /// Gets or sets the version filter.
        /// </summary>
        /// <value>The version filter.</value>
        public string Version { get; set; }

        /// <summary>
        /// Gets the filter types associated with this request.
        /// </summary>
        /// <returns>The filter types associated with this request.</returns>
        public List<FilterTypes> GetFilterTypesForRequest()
        {
            var filters = new List<FilterTypes>();
            if (Protocol != null)
            {
                filters.Add(FilterTypes.ProtocolFilter);
            }
            if (!string.IsNullOrEmpty(Continent))
            {
                filters.Add(FilterTypes.ContinentFilter);
            }
            if (!string.IsNullOrEmpty(CountryCode))
            {
                filters.Add(FilterTypes.CountryCodeFilter);
            }
            if (!string.IsNullOrEmpty(Map))
            {
                filters.Add(FilterTypes.MapFilter);
            }
            if (!string.IsNullOrEmpty(Os))
            {
                filters.Add(FilterTypes.OsFilter);
            }
            if (!string.IsNullOrEmpty(GameType))
            {
                filters.Add(FilterTypes.GameTypeFilter);
            }
            // User specified a 'has players' filter (either true or false) in URL.
            if (HasPlayers != null)
            {
                filters.Add(HasPlayers == true
                    ? FilterTypes.PlayersTrueFilter
                    : FilterTypes.PlayersFalseFilter);
            }
            // User specified an 'is not full' filter (either true or false) in URL.
            if (IsNotFull != null)
            {
                filters.Add(IsNotFull == true ? FilterTypes.NotFullTrueFilter : FilterTypes.NotFullFalseFilter);
            }
            // User specified a 'server has (or doesn't have) password' filter in URL.
            if (HasPassword != null)
            {
                filters.Add(FilterTypes.PasswordFilter);
            }
            // User specified a 'server has (or doesn't have) VAC protection filter in URL.
            if (HasVac != null)
            {
                filters.Add(FilterTypes.VacFilter);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                filters.Add(FilterTypes.VersionFilter);
            }

            return filters;
        }
    }
}
