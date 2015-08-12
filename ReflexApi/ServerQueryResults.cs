 // ReSharper disable InconsistentNaming

namespace ReflexAPI
{
    using System.Collections.Generic;
    using SteamData;

    /// <summary>
    /// Wrapper class containing the actual lists of servers to be returned in a ServerQueryResponse
    /// as a result of a ServerQueryRequest.
    /// </summary>
    public class ServerQueryResults
    {
        /// <summary>
        /// Gets or sets the list of servers (with ports) that failed to query.
        /// </summary>
        /// <value>The list of servers (with ports) that failed to query.</value>
        public List<string> failedServers { get; set; }

        /// <summary>
        /// Gets or sets the query time in a human-readable format.
        /// </summary>
        /// <value>
        /// The query time in a human-readable format.
        /// </value>
        public string queryTime { get; set; }

        /// <summary>
        /// Gets or sets the query timestamp.
        /// </summary>
        /// <value>
        /// The query timestamp.
        /// </value>
        public double queryTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the servers that are to be returned.
        /// </summary>
        /// <value>The servers that are to be returned.</value>
        public List<ServerData> servers { get; set; }

        /// <summary>
        /// Gets or sets the servers that were skipped due to being unindexed (not contained in our
        /// master server list).
        /// </summary>
        /// <value>The servers that were skipped due to being unindexed.</value>
        public List<string> unindexedServers { get; set; }

    }
}
