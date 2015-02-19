using System.Collections.Generic;
using ReflexApi.SteamData;

// ReSharper disable InconsistentNaming

namespace Reflexapi.Models
{
    /// <summary>
    ///     Class that contains the data to be sent in the response to the user's server query request.
    /// </summary>
    /// <remarks>
    /// Ignore ReSharper warnings about capitalization since JSON
    /// responses are not typically capitalized in APIs.
    /// </remarks>
    public class ServerQueryResponse
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
        ///     Gets or sets the servers that are to be returned.
        /// </summary>
        /// <value>
        ///     The servers that are to be returned.
        /// </value>
        public List<ServerData> servers { get; set; }
    }
}