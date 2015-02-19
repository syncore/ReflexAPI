using Reflexapi.Models;
using ServiceStack;

namespace ReflexApi.Models
{
    /// <summary>
    ///     Class responsible for accepting the user's request (specified in URL) to query a server.
    ///     <remarks>
    ///         The PrivateQuery is intended for testing purposes and trusted users.
    ///         Currently this is not used publicly. For now, if used, it will be used for individual
    ///         projects (i.e. bots) and/or will be given to individuals who request it. This is different from
    ///         the standard ServerQuery which is limited to querying servers that exist in the master server list.
    ///     </remarks>
    /// </summary>
    [Route("/unlistedquery", "GET")]
    public class PrivateQueryRequest : IReturn<PrivateQueryResponse>
    {
        // Filters in the URL (not case sensitive despite capitalization of properties below)
        // Query server: http://the.webserver.com/unlistedquery?host=hostname1,hostname2,hostname3...hostnameN,&port=portnumber1,portnumber2,portnumber3..portnumberN

        /// <summary>
        ///     Gets or sets the hostname filter.
        /// </summary>
        /// <value>
        ///     The hostname filter.
        /// </value>
        public string Host { get; set; }

        /// <summary>
        ///     Gets or sets the port filter.
        /// </summary>
        /// <value>
        ///     The port filter.
        /// </value>
        public string Port { get; set; }
    }
}