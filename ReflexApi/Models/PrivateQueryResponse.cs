using System.Collections.Generic;
using ReflexAPI.SteamData;

// ReSharper disable InconsistentNaming

namespace ReflexAPI.Models
{
    /// <summary>
    ///     Class that contains the data to be sent in the response to the user's server query request.
    /// </summary>
    /// <remarks>
    ///     The PrivateQuery is intended for testing purposes and trusted users.
    ///     Ignore ReSharper warnings about capitalization since JSON
    ///     responses are not typically capitalized in APIs.
    /// </remarks>
    public class PrivateQueryResponse
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
        ///     The number of servers that failed to query.
        /// </value>
        public int? failedCount { get; set; }

        /// <summary>
        ///     Gets or sets the servers that failed to query.
        /// </summary>
        /// <value>
        ///     The servers that failed to query.
        /// </value>
        public List<string> failedServers { get; set; }

        /// <summary>
        ///     Gets or sets the servers that are to be returned.
        /// </summary>
        /// <value>
        ///     The servers that are to be returned.
        /// </value>
        public List<ServerData> servers { get; set; }
    }
}