using System;
using System.Collections.Generic;
using System.Linq;
using ReflexAPI.Enum;
using ReflexAPI.Models;
using ReflexAPI.SteamData;
using ServiceStack;

namespace ReflexAPI.Services
{
    /// <summary>
    ///     Class responsible for handling a user's server list request.
    ///     This class will generally receive a user's server list request, determine if the request
    ///     has any filters, and return the appropriate response to the user.
    /// </summary>
    public class ServerListService : Service
    {
        /// <summary>
        ///     Receives and processes the user's API request if possible.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The appropriate response based on the type of request received.</returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public object Get(ServerListRequest request)
        {
            var filters = request.GetFilterTypesForRequest();
            // Send back the appropriate response.
            return filters.Count > 0 ? HandleFilteredRequest(filters, request) : HandleUnfilteredRequest();
        }

        /// <summary>
        ///     Generates the filtered server list.
        /// </summary>
        /// <param name="filterTypes">The filter types applicable for the given request.</param>
        /// <param name="previousFilter">The previous (initial) unfiltered list.</param>
        /// <param name="request">The request.</param>
        /// <returns>The list of servers that meet the specified criteria.</returns>
        private List<ServerData> GenerateFilteredList(IEnumerable<FilterTypes> filterTypes,
            List<ServerData> previousFilter, ServerListRequest request)
        {
            //TODO: REFACTOR
            var filteredServers = new List<ServerData>();

            foreach (var filter in filterTypes)
            {
                if (filter == FilterTypes.CountryCodeFilter)
                {
                    filteredServers =
                        (previousFilter.Where(
                            s =>
                                s.countryCode.Equals(request.CountryCode,
                                    StringComparison.InvariantCultureIgnoreCase)).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.GameTypeFilter)
                {
                    filteredServers =
                        (previousFilter.Where(
                            s =>
                                s.gametype.Equals(request.GameType,
                                    StringComparison.InvariantCultureIgnoreCase)).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.MapFilter)
                {
                    filteredServers =
                        (previousFilter.Where(
                            s => s.map.Equals(request.Map, StringComparison.InvariantCultureIgnoreCase))
                            .ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.NotFullFalseFilter)
                {
                    filteredServers = (previousFilter.Where(s => (s.playerCount != s.maxPlayers) ||
                                                                 (s.playerCount == s.maxPlayers)).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.NotFullTrueFilter)
                {
                    filteredServers = (previousFilter.Where(s => s.playerCount != s.maxPlayers).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.OsFilter)
                {
                    filteredServers =
                        (previousFilter.Where(
                            s => s.os.Equals(request.Os, StringComparison.InvariantCultureIgnoreCase))
                            .ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.PasswordFilter)
                {
                    filteredServers =
                        (previousFilter.Where(s => s.requiresPassword == request.HasPassword).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.PlayersFalseFilter)
                {
                    filteredServers = (previousFilter.Where(s => s.playerCount == 0).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.PlayersTrueFilter)
                {
                    filteredServers = (previousFilter.Where(s => s.playerCount > 0).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.ProtocolFilter)
                {
                    filteredServers = (previousFilter.Where(s => s.protocol == request.Protocol).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.VacFilter)
                {
                    filteredServers =
                        (previousFilter.Where(s => s.hasVacProtection == request.HasVac).ToList());
                    previousFilter = filteredServers;
                    continue;
                }
                if (filter == FilterTypes.VersionFilter)
                {
                    filteredServers =
                        (previousFilter.Where(s => s.version.Contains(request.Version)).ToList());
                }
            }

            return filteredServers;
        }

        /// <summary>
        ///     Handles the request if it contains any user specified filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     A list of servers that meet the requirements of the request.
        /// </returns>
        private ServerListResponse HandleFilteredRequest(List<FilterTypes> filters, ServerListRequest request)
        {
            if (ServerList.AllServers == null)
            {
                // Return default response with generic message & empty server list instead of ServiceStack's
                // serialized JSON information that contains the .NET exception info.
                return new ServerListResponse
                {
                    msg = "Init in progress.",
                    count = 0,
                    servers = new List<ServerData>()
                };
            }

            var unfiltered = ServerList.AllServers;
            var filteredServers = GenerateFilteredList(filters, unfiltered, request);

            return new ServerListResponse
            {
                count = filteredServers.Count,
                servers = filteredServers
            };
        }

        /// <summary>
        ///     Handles the request if it does not contain any user specified filters.
        /// </summary>
        /// <returns></returns>
        private ServerListResponse HandleUnfilteredRequest()
        {
            if (ServerList.AllServers == null)
            {
                // Return default response with generic message & empty server list instead of ServiceStack's
                // serialized JSON information that contains the .NET exception info.
                return new ServerListResponse
                {
                    msg = "Init in progress.",
                    count = 0,
                    servers = new List<ServerData>()
                };
            }

            return new ServerListResponse
            {
                count = ServerList.AllServers.Count,
                servers = ServerList.AllServers,
                failedCount = ServerList.FailedServers.Count,
                failedServers = ServerList.FailedServers
            };
        }
    }
}