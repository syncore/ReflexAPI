 // ReSharper disable InconsistentNaming

namespace ReflexAPI.SteamData
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Model for Steam data that represents a Reflex server returned from Valve's master server.
    /// </summary>
    /// <remarks>
    /// Ignore ReSharper warnings about capitalization since JSON responses are not typically
    /// capitalized in APIs.
    /// </remarks>
    public class ServerData
    {
        /// <summary>
        /// Gets or sets the number of bots on the server.
        /// </summary>
        /// <value>The number of bots on the server.</value>
        public int? bots { get; set; }

        /// <summary>
        /// Gets or sets the server's continent.
        /// </summary>
        /// <value>
        /// The server's continent.
        /// </value>
        public string continent { get; set; }
        
        /// <summary>
        /// Gets or sets the two letter country code for the server.
        /// </summary>
        /// <value>The server's two letter country code.</value>
        public string countryCode { get; set; }

        /// <summary>
        /// Gets or sets name of the country in which the server is hosted.
        /// </summary>
        /// <value>The name of the country in which the server is hosted.</value>
        public string countryName { get; set; }

        /// <summary>
        /// Gets or sets the game the server is running.
        /// </summary>
        /// <value>The game the server is running.</value>
        public string game { get; set; }

        /// <summary>
        /// Gets or sets the gametype the server is running.
        /// </summary>
        /// <value>The gametype.</value>
        /// <remarks>This is a custom value derived from the keywords.</remarks>
        public string gametype { get; set; }

        /// <summary>
        /// Gets or sets whether the server has vac protection enabled.
        /// </summary>
        /// <value>Whether the server has vac protection enabled.</value>
        public bool? hasVacProtection { get; set; }

        /// <summary>
        /// Gets or sets the server's identifier.
        /// </summary>
        /// <value>The server's identifier.</value>
        public int? id { get; set; }

        /// <summary>
        /// Gets or sets the server's ip.
        /// </summary>
        /// <value>The server's ip.</value>
        public string ip { get; set; }

        /// <summary>
        /// Gets or sets the server's keywords.
        /// </summary>
        /// <value>The server's keywords.</value>
        public string keywords { get; set; }

        /// <summary>
        /// Gets or sets the server's map.
        /// </summary>
        /// <value>The server's map.</value>
        public string map { get; set; }

        /// <summary>
        /// Gets or sets the maximum players on the server.
        /// </summary>
        /// <value>The maximum players on the server.</value>
        public int? maxPlayers { get; set; }

        /// <summary>
        /// Gets or sets the server's operating system.
        /// </summary>
        /// <value>The server's operating system.</value>
        public string os { get; set; }

        /// <summary>
        /// Gets or sets the server's player count.
        /// </summary>
        /// <value>The server's player count.</value>
        public int? playerCount { get; set; }

        /// <summary>
        /// Gets or sets the players on the server.
        /// </summary>
        /// <value>The players on the server.</value>
        public List<PlayerData> players { get; set; }

        /// <summary>
        /// Gets or sets the server's (game, not steam) port.
        /// </summary>
        /// <value>The server's (game, not steam) port.</value>
        public int? port { get; set; }

        //public string SourceTvName { get; set; }

        /// <summary>
        /// Gets or sets the server's protocol.
        /// </summary>
        /// <value>The server's protocol.</value>
        public int? protocol { get; set; }

        //public int? sourceTvPort { get; set; }

        /// <summary>
        /// Gets or sets whether the requires password.
        /// </summary>
        /// <value>Whether the server requires a password.</value>
        public bool? requiresPassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name of the server.</value>
        public string serverName { get; set; }

        /// <summary>
        /// Gets or sets the type of server.
        /// </summary>
        /// <value>The type of server.</value>
        public string serverType { get; set; }

        //public string folder { get; set; }

        /// <summary>
        /// Gets or sets the game's steam identifier.
        /// </summary>
        /// <value>The game's steam identifier.</value>
        public ulong? steamIdGame { get; set; }

        /// <summary>
        /// Gets or sets the server's signed Steam identifier.
        /// </summary>
        /// <value>The server's signed Steam identifier.</value>
        public long? steamIdServer { get; set; }

        /// <summary>
        /// Gets or sets the server's Steam query port.
        /// </summary>
        /// <value>The server's Steam query port.</value>
        public int? steamPort { get; set; }

        /// <summary>
        /// Gets or sets the server's unsigned Steam identifier.
        /// </summary>
        /// <value>The server's unsigned Steam identifier.</value>
        public ulong? unsignedServerSteamId { get; set; }

        /// <summary>
        /// Gets or sets the server's version.
        /// </summary>
        /// <value>The server's version.</value>
        public string version { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            // For testing purposes...

            var playerBuilder = new StringBuilder();
            foreach (var player in players)
            {
                playerBuilder.Append(string.Format("Name: {0}, Score: {1}, Duration: {2} --- ", player.name,
                    player.score, player.connectedFor));
            }

            return
                string.Format(
                    "Got server: [IP]: {0} | [PORT]: {1} | [PROTOCOL]: {2} | [MAP]: {3} | [GAME]: {4} | [ID]: {5} |" +
                    " [PLAYERCOUNT]: {6} | [MAXPLAYERS]: {7} | [SERVERTYPE]: {8} | [PASSWORD REQUIRED]: {9} | [VAC ENABLED]:" +
                    " {10} | [VERSION]: {11} | [BOTS]: {12} | [SERVER OS]: {13} | [UNSIGNED STEAM ID]: {14} | [KEYWORDS]:" +
                    " {15} | [GAME STEAM ID]: {16} | [PLAYERS]: {17}",
                    ip, port, protocol, map, game, id, playerCount, maxPlayers, serverType, requiresPassword,
                    hasVacProtection, version, bots, os, steamIdServer, keywords,
                    steamIdGame, playerBuilder);
        }
    }
}
