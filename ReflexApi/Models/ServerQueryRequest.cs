using Reflexapi.Models;
using ServiceStack;

namespace ReflexApi.Models
{
    /// <summary>
    ///     Class responsible for accepting the user's request (specified in URL) to query a server.
    ///     <remarks>
    ///         Note, the Route attribute specifies the part of the URL that
    ///         this request is responsible for. It can be defined at the class level, as done here,
    ///         or in a container in the ReflexApiHost.cs file.
    ///     </remarks>
    /// </summary>
    [Route("/queryserver", "GET")]
    public class ServerQueryRequest : IReturn<ServerQueryResponse>
    {
        // Filters in the URL (not case sensitive despite capitalization of properties below)
        // Format (10 MAX hosts. # of hosts and ports must match as well)
        // Query server:
        // http://the.webserver.com/queryserver?host=hostname1,hostname2,hostname3...hostname10,&port=portnumber1,portnumber2,portnumber3..portnumber10

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