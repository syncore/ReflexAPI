using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using log4net;
using ReflexAPI.SteamData;
using ReflexAPI.Util;
using SteamInfo.MasterServer;
using SteamInfo.MasterServer.Filters;
using SteamInfo.Server;
using Environment = SteamInfo.Server.Environment;

namespace ReflexAPI
{
    /// <summary>
    ///     Class responsible for querying the server information from Steam's master server.
    /// </summary>
    public class ServerQueryProcessor
    {
        private const int ReceiveTimeoutMsec = 3300;
        private readonly HostCountryRetriever _countryRetriever;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerQueryProcessor" /> class.
        /// </summary>
        public ServerQueryProcessor()
        {
            _countryRetriever = new HostCountryRetriever();
        }

        /// <summary>
        ///     Gets or sets the total servers.
        /// </summary>
        /// <value>
        ///     The total servers.
        /// </value>
        public int TotalServers { get; set; }

        /// <summary>
        ///     Gets all of the servers from the Steam Master server.
        /// </summary>
        /// <returns><c>true</c> if Valve's master server could be contacted, otherwise <c>false</c>.</returns>
        public bool GetAllServers()
        {
            var masterServerQuery = new MasterServerQuery();
            List<IPEndPoint> serversReceivedFromMaster;
            var serversToReturn = new List<ServerData>();

            try
            {
                var filter = new Filter {Region = Region.RestOfTheWorld, Game = Game.Reflex};
                serversReceivedFromMaster = masterServerQuery.GetServers(filter, ReceiveTimeoutMsec);
            }
            catch (Exception ex)
            {
                LoggerUtil.LogInfoAndDebug(
                    string.Format("Error when contacting Valve's Steam Master server. Exception: {0}"
                        , ex.Message), LogClassType);
                return false;
            }

            var failedServers = new List<string>();
            TotalServers = serversReceivedFromMaster.Count;
            var queryNum = 0;

            LoggerUtil.LogInfoAndDebug(string.Format("Total servers to query: {0}", TotalServers),
                LogClassType);

            foreach (var server in serversReceivedFromMaster)
            {
                var sd = CreateServerDataInfo(server);
                if (sd != null)
                {
                    serversToReturn.Add(sd);
                }
                else
                {
                    failedServers.Add(string.Format("{0}:{1}", server.Address, server.Port));
                }
                queryNum++;

                Logger.Debug(string.Format("Queried server {0}/{1}", queryNum, TotalServers));
            }

            ServerList.AllServers = serversToReturn;
            ServerList.FailedServers = failedServers;

            Logger.Info(string.Format("Finished query of {0}/{1} servers; total failed: {2}", queryNum,
                TotalServers, failedServers.Count));

            return true;
        }

        /// <summary>
        ///     Queries the specified servers.
        /// </summary>
        /// <param name="serversToQuery">The servers to query.</param>
        /// <returns>The results of the query as a <see cref="ServerQueryResults" /> object.</returns>
        public ServerQueryResults QueryServers(List<IPEndPoint> serversToQuery)
        {
            var results = new ServerQueryResults();
            var successful = new List<ServerData>();
            var failed = new List<string>();

            foreach (var server in serversToQuery)
            {
                var queried = CreateServerDataInfo(server);
                if (queried != null)
                {
                    successful.Add(queried);
                }
                else
                {
                    failed.Add(string.Format("{0}:{1}", server.Address, server.Port));
                    LoggerUtil.LogInfoAndDebug(
                        string.Format("Manual user server query failed for {0}:{1}", server.Address,
                            server.Port), LogClassType);
                }
            }
            results.servers = successful;
            results.failedServers = failed;
            return results;
        }

        /// <summary>
        ///     Queries a single specified server.
        /// </summary>
        /// <param name="serverToQuery">The server to query.</param>
        /// <returns>The results of the query as a <see cref="ServerQueryResults" /> object.</returns>
        public ServerQueryResults QueryServers(IPEndPoint serverToQuery)
        {
            var results = new ServerQueryResults();
            var successful = new List<ServerData>();
            var failed = new List<string>();
            var queried = CreateServerDataInfo(serverToQuery);
            if (queried != null)
            {
                successful.Add(queried);
            }
            else
            {
                failed.Add(string.Format("{0}:{1}", serverToQuery.Address, serverToQuery.Port));
                LoggerUtil.LogInfoAndDebug(
                    string.Format("Manual user server query failed for {0}:{1}", serverToQuery.Address,
                        serverToQuery.Port), LogClassType);
            }

            results.servers = successful;
            results.failedServers = failed;
            return results;
        }

        /// <summary>
        ///     Creates the server data information (object) for a given ip and port.
        /// </summary>
        /// <param name="serverAddress">The server address.</param>
        /// <returns>
        ///     The server's information as a <see cref="ServerData" /> object if the query was successful, otherwise returns
        ///     null.
        /// </returns>
        private ServerData CreateServerDataInfo(IPEndPoint serverAddress)
        {
            var query = new ServerQuery(serverAddress);
            ServerData sData;
            try
            {
                var serverInfo = query.GetServerInfo(ReceiveTimeoutMsec);
                var playerInfo = query.GetPlayers(ReceiveTimeoutMsec);
                var players = playerInfo.Players.Select(player => new PlayerData
                {
                    name = player.Name,
                    score = player.Score,
                    connectedFor = Math.Round(player.Duration, 2)
                }).ToList();

                // Query the SQLite DB for the country info based on the IP
                var countryInfo = _countryRetriever.GetCountryInfo(serverAddress);

                // Recent 0.33+ builds have added data to the keywords
                var gametype = serverInfo.ExtraData.Keywords.Split('|');
                
                var sd = new ServerData
                {
                    serverName = serverInfo.Name,
                    ip = serverAddress.Address.ToString(),
                    countryName = countryInfo.countryName,
                    countryCode = countryInfo.countryCode,
                    protocol = serverInfo.Protocol,
                    map = serverInfo.Map,
                    game = serverInfo.Game,
                    playerCount = serverInfo.Players,
                    maxPlayers = serverInfo.MaxPlayers,
                    serverType =
                        (serverInfo.ServerType == ServerType.Dedicated ? "dedicated" : "listen"),
                    requiresPassword = serverInfo.RequiresPassword,
                    hasVacProtection = serverInfo.IsVacProtected,
                    keywords = serverInfo.ExtraData.Keywords,
                    version = serverInfo.Version,
                    bots = serverInfo.Bots,
                    os =
                        (serverInfo.Environment == Environment.Linux
                            ? "linux"
                            : "windows"),
                    port = serverInfo.ExtraData.Port,
                    steamIdServer = serverInfo.ExtraData.SignedServerSteamId,
                    // The very first server builds ("Reflex Build # ##") do not list keywords, so this is needed
                    gametype =
                        ((string.IsNullOrEmpty(serverInfo.ExtraData.Keywords))
                            ? ""
                            : gametype[0].ToUpper()),
                    steamIdGame = serverInfo.ExtraData.GameSteamId,
                    steamPort = serverAddress.Port,
                    players = players
                };
                sData = sd;
            }
            catch (Exception ex)
            {
                LoggerUtil.LogInfoAndDebug(
                    string.Format("ERROR/TIMEOUT for server query: {0}:{1} - Exception: {2}",
                        serverAddress.Address, serverAddress.Port, ex.Message), LogClassType);
                sData = null;
            }

            return sData;
        }

        private static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly ILog Logger = LogManager.GetLogger(LogClassType);
    }
}