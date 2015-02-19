using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using log4net;
using ReflexApi.SteamData;
using ReflexApi.Util;
using SteamInfo.MasterServer;
using SteamInfo.MasterServer.Filters;
using SteamInfo.Server;
using Environment = SteamInfo.Server.Environment;

namespace ReflexApi
{
    /// <summary>
    ///     Class responsible for querying the server information from Steam's master server.
    /// </summary>
    public class ServerQueryProcessor
    {
        private const int ReceiveTimeoutMsec = 2000;
        private static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly ILog Logger = LogManager.GetLogger(LogClassType);
        private readonly HostCountryRetriever _countryRetriever;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerQueryProcessor"/> class.
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
        /// Gets all of the servers from the Steam Master server.
        /// </summary>
        /// <returns><c>true</c> if Valve's master server could be contacted, otherwise <c>false</c>.</returns>
        public bool GetAllServers()
        {
            var masterServerQuery = new MasterServerQuery();
            List<IPEndPoint> serversReceivedFromMaster;
            var serversToReturn = new List<ServerData>();

            try
            {
                var filter = new Filter { Region = Region.RestOfTheWorld, Game = Game.Reflex };
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
            int queryNum = 0;

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
        /// <returns>A list of <see cref="ServerData"/> objects containing the successfully queried server information.</returns>
        public List<ServerData> QueryServers(List<IPEndPoint> serversToQuery)
        {
            var successfulQueries = new List<ServerData>();
            foreach (var server in serversToQuery)
            {
                var queried = CreateServerDataInfo(server);
                if (queried != null)
                {
                    successfulQueries.Add(queried);
                }
                else
                {
                    LoggerUtil.LogInfoAndDebug(
                        string.Format("Manual user server query failed for {0}:{1}", server.Address,
                            server.Port), LogClassType);
                }
            }
            return successfulQueries;
        }

        /// <summary>
        ///     Queries a single specified server.
        /// </summary>
        /// <param name="serverToQuery">The server to query.</param>
        /// <returns>A list of <see cref="ServerData"/> objects containing the successfully queried server information.</returns>
        public List<ServerData> QueryServers(IPEndPoint serverToQuery)
        {
            var successfulQueries = new List<ServerData>();
            var queried = CreateServerDataInfo(serverToQuery);
            if (queried != null)
            {
                successfulQueries.Add(queried);
            }
            else
            {
                LoggerUtil.LogInfoAndDebug(
                    string.Format("Manual user server query failed for {0}:{1}", serverToQuery.Address,
                        serverToQuery.Port), LogClassType);
            }

            return successfulQueries;
        }

        /// <summary>
        ///     Creates the server data information (object) for a given ip and port.
        /// </summary>
        /// <param name="serverAddress">The server address.</param>
        /// <returns>The server's information as a <see cref="ServerData"/> object if the query was successful, otherwise returns null.</returns>
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
                    version = serverInfo.Version,
                    bots = serverInfo.Bots,
                    os =
                        (serverInfo.Environment == Environment.Linux
                            ? "linux"
                            : "windows"),
                    port = serverInfo.ExtraData.Port,
                    steamIdServer = serverInfo.ExtraData.SignedServerSteamId,
                    // The very first server builds ("Reflex Build # ##") do not list keywords, so this is needed
                    gametype = ((string.IsNullOrEmpty(serverInfo.ExtraData.Keywords)) ? "" : serverInfo.ExtraData.Keywords),
                    //keywords = serverInfo.ExtraData.Keywords,
                    steamIdGame = serverInfo.ExtraData.GameSteamId,
                    steamPort = serverAddress.Port,
                    players = players,
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
    }
}