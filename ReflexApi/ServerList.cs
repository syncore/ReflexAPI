namespace ReflexAPI
{
    using System.Collections.Generic;
    using SteamData;

    /// <summary>
    /// Static class that holds an in-memory representation of the current server list and servers
    /// that failed to query. <remarks> Persist so multiple user requests do not initiate a new
    /// server list request. </remarks>
    /// </summary>
    public static class ServerList
    {
        /// <summary>
        /// Gets all servers.
        /// </summary>
        /// <value>All servers.</value>
        public static List<ServerData> AllServers { get; set; }

        /// <summary>
        /// Gets the list of server ip addresses and ports that failed to query.
        /// </summary>
        /// <value>The failed servers.</value>
        public static List<string> FailedServers { get; set; }

        /// <summary>
        /// Gets or sets the query time.
        /// </summary>
        /// <value>
        /// The query time.
        /// </value>
        public static string QueryTime { get; set; }

        /// <summary>
        /// Gets or sets the query time stamp.
        /// </summary>
        /// <value>
        /// The query time stamp.
        /// </value>
        public static double QueryTimeStamp { get; set; }
    }
}
