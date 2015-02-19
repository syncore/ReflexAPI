using System.Collections.Generic;
using ReflexApi.SteamData;
using ServiceStack;

// ReSharper disable InconsistentNaming

namespace Reflexapi.Models
{
    /// <summary>
    /// Class that contains the data to be sent in the response to the user's server list request.
    /// </summary>
    /// <remarks>
    ///    Ignore ReSharper warnings about capitalization since JSON
    ///     responses are not typically capitalized in APIs.
    /// </remarks>
    public class ServerListResponse
    {
        /// <summary>
        ///     Gets or sets the number of servers that are to be returned.
        /// </summary>
        /// <value>
        ///     The number of servers that are to be returned.
        /// </value>
        public int? count { get; set; }

        /// <summary>
        ///     Gets or sets the error, if any.
        /// </summary>
        /// <value>
        ///     The error, if any.
        /// </value>
        public string error { get; set; }

        /// <summary>
        ///     Gets or sets the number of servers that failed to query.
        /// </summary>
        /// <value>
        ///     the number of servers that failed to query.
        /// </value>
        public int? failedCount { get; set; }

        /// <summary>
        ///     Gets or sets the list of servers (with ports) that failed to query.
        /// </summary>
        /// <value>
        ///     The list of servers (with ports) that failed to query.
        /// </value>
        public List<string> failedServers { get; set; }

        /// <summary>
        /// Gets or sets a generic message.
        /// </summary>
        /// <value>
        /// The generic message.
        /// </value>
        public string msg { get; set; }

        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        /// <value>
        /// The response status.
        /// </value>
        public ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        ///     Gets or sets the servers that are to be returned.
        /// </summary>
        /// <value>
        ///     The servers that are to be returned.
        /// </value>
        public List<ServerData> servers { get; set; }
    }
}