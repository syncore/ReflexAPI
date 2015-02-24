namespace ReflexAPI
{
    /// <summary>
    ///     Class that contains the responses (typically errors) to individual API server queries.
    /// </summary>
    public sealed class QueryResponseCode
    {
        public static readonly QueryResponseCode HostLimitExceededError =
            new QueryResponseCode("Too many hosts were specified.",
                "Failed manual user server query request: user specified too many hosts.");

        public static readonly QueryResponseCode HostNotIndexedError =
            new QueryResponseCode("Host has not been indexed.",
                "Failed manual user server query request: user's query contained host that has not been indexed.");

        public static readonly QueryResponseCode HostNotSpecifiedError =
            new QueryResponseCode("Hostname not specified.",
                "Failed manual user server query request: user did not specify hostname.");

        public static readonly QueryResponseCode HostPortsInvalidError =
            new QueryResponseCode("Hosts and/or ports are invalid.",
                "Failed manual user server query request: user's query contained invalid hostnames and/or ports.");

        public static readonly QueryResponseCode HostPortsMismatchError =
            new QueryResponseCode("Number of hosts must match number of ports.",
                "Failed manual user server query request: # of hosts did not match # of ports.");

        public static readonly QueryResponseCode PortNotSpecifiedError =
            new QueryResponseCode("Port not specified",
                "Failed manual user server query request: user did not specify port.");

        public static readonly QueryResponseCode ResolutionError =
            new QueryResponseCode("Error occurred while attempting to resolve host.",
                "Failed manual user server query request: error resolving user specified host/ip.");

        // In the event of a successful query, the results are shown, not the messages.
        public static readonly QueryResponseCode Success = new QueryResponseCode("Success",
            "Successful query.");

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryResponseCode" /> class.
        /// </summary>
        /// <param name="userMessage">The (error) message to display to the user via the API.</param>
        /// <param name="loggerMessage">The (error) message to log to the internal logger.</param>
        private QueryResponseCode(string userMessage, string loggerMessage)
        {
            UserMessage = userMessage;
            LoggerMessage = loggerMessage;
        }

        /// <summary>
        ///     Gets the (error) message to log to the internal logger.
        /// </summary>
        /// <value>
        ///     The (error) message to log to the internal logger.
        /// </value>
        public string LoggerMessage { get; private set; }

        /// <summary>
        ///     Gets the (error) message to display to the user via the API.
        /// </summary>
        /// <value>
        ///     The (error) message to display to the user via the API
        /// </value>
        public string UserMessage { get; private set; }
    }
}