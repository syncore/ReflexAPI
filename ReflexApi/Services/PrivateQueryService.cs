using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Reflexapi.Models;
using ReflexApi.Models;
using ReflexApi.SteamData;
using ReflexApi.Util;
using ServiceStack;

namespace ReflexApi.Services
{
    /// <summary>
    ///     Class responsible for handling a user's (unlisted) server query request.
    /// </summary>
    /// <remarks>
    ///     The PrivateQuery is primarily intended for testing purposes.
    /// </remarks>
    public class PrivateQueryService : Service
    {
        private const int MaxHostsAllowed = 16;
        private static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;

        /// <summary>
        ///     Receives and processes the user's API request if possible.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The appropriate response based on the type of request received.</returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public object Get(PrivateQueryRequest request)
        {
            LoggerUtil.LogInfoAndDebug(
                string.Format(
                    "Received manual user UNLISTED API server query request for hostname(s): {0} and port(s): {1}",
                    request.Host, request.Port), LogClassType);

            // Check if user specified the server's hostname.
            if (string.IsNullOrEmpty(request.Host))
            {
                return ReturnInvalidResponseDetails(QueryResponseCode.HostNotSpecifiedError);
            }
            // Check if user specified the server's port.
            if (string.IsNullOrEmpty(request.Port))
            {
                return ReturnInvalidResponseDetails(QueryResponseCode.PortNotSpecifiedError);
            }
            // Multi-query
            if ((request.Host.Contains(',') && (request.Port.Contains(','))))
            {
                return HandleMultiServerQuery(request);
            }

            return HandleSingleServerQuery(request);
        }

        /// <summary>
        ///     Checks the hostname and port validity.
        /// </summary>
        /// <param name="hostnames">The hostnames.</param>
        /// <param name="ports">The ports.</param>
        /// <returns>The <see cref="QueryResponseCode" /> that represents the outcome of the validity check.</returns>
        private QueryResponseCode CheckMultipleHostValidity(string[] hostnames, string[] ports)
        {
            if (hostnames.Length != ports.Length)
            {
                return QueryResponseCode.HostPortsMismatchError;
            }
            if (hostnames.Length > MaxHostsAllowed)
            {
                return QueryResponseCode.HostLimitExceededError;
            }
            if (!HostsPortsValid(hostnames, ports))
            {
                return QueryResponseCode.HostPortsInvalidError;
            }
            return QueryResponseCode.Success;
        }

        /// <summary>
        ///     Handles a server query request containing multiple hosts.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A list of servers that meet the requirements of the request or an error.</returns>
        private PrivateQueryResponse HandleMultiServerQuery(PrivateQueryRequest request)
        {
            var hostnames = request.Host.Trim().Split(',');
            var ports = request.Port.Trim().Split(',');
            var validationResult = CheckMultipleHostValidity(hostnames, ports);

            // Final validation
            if (validationResult != QueryResponseCode.Success)
            {
                return ReturnInvalidResponseDetails(validationResult);
            }

            var toQuery = new List<IPEndPoint>();
            var port = 0;
            for (var i = 0; i < hostnames.Length; i++)
            {
                for (var j = 0; j < ports.Length; j++)
                {
                    int.TryParse(ports[j], out port);
                }
                if (port != 0)
                {
                    try
                    {
                        var ips = Dns.GetHostAddresses(hostnames[i]);
                        if (ips.Length > 0)
                        {
                            toQuery.Add(new IPEndPoint(ips[0], port));
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerUtil.LogInfoAndDebug(
                            string.Format(
                                "Manual multi-servery query: error resolving user specified host/ip: {0}:{1}. Exception: {2}"
                                , request.Host, request.Port, ex.Message), LogClassType);
                        return ReturnInvalidResponseDetails(QueryResponseCode.ResolutionError);
                    }
                }
            }

            var sqp = new ServerQueryProcessor();
            var serversToReturn = sqp.QueryServers(toQuery);

            return new PrivateQueryResponse {count = serversToReturn.Count, servers = serversToReturn};
        }

        /// <summary>
        ///     Handles a single server query.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response containing the server information or an error.</returns>
        private PrivateQueryResponse HandleSingleServerQuery(PrivateQueryRequest request)
        {
            if (!HostsPortsValid(request.Host, request.Port))
            {
                return ReturnInvalidResponseDetails(QueryResponseCode.HostPortsInvalidError);
            }

            var sqp = new ServerQueryProcessor();
            int prt;
            var serversToReturn = new List<ServerData>();

            if (int.TryParse(request.Port, out prt))
            {
                try
                {
                    var ips = Dns.GetHostAddresses(request.Host);
                    if (ips.Length > 0)
                    {
                        serversToReturn = sqp.QueryServers(new IPEndPoint(ips[0], prt));
                    }
                    else
                    {
                        return ReturnInvalidResponseDetails(QueryResponseCode.ResolutionError);
                    }
                }
                catch (Exception ex)
                {
                    LoggerUtil.LogInfoAndDebug(
                        string.Format("Error resolving user specified host/ip: {0}:{1} Exception: {2}"
                            , request.Host, request.Port, ex.Message), LogClassType);
                    return ReturnInvalidResponseDetails(QueryResponseCode.ResolutionError);
                }
            }

            return new PrivateQueryResponse {count = serversToReturn.Count, servers = serversToReturn};
        }

        /// <summary>
        ///     Checks whether the given hostnames and ports are valid.
        /// </summary>
        /// <param name="hostnames">The hostnames to check.</param>
        /// <param name="ports">The ports to check.</param>
        /// <returns><c>true</c> if all of the hosts and ports are valid, otherwise <c>false</c>if any are invalid.</returns>
        private bool HostsPortsValid(string[] hostnames, string[] ports)
        {
            var portsValid = false;
            var hostsValid = false;
            for (var i = 0; i < hostnames.Length; i++)
            {
                hostsValid = (Uri.CheckHostName(hostnames[i]) != UriHostNameType.Unknown);
            }
            for (var i = 0; i < ports.Length; i++)
            {
                int portNum;
                if (!int.TryParse(ports[i], out portNum))
                {
                    portsValid = false;
                }
                else if (portNum > 65535)
                {
                    portsValid = false;
                }
                else
                {
                    portsValid = true;
                }
            }
            if (!hostsValid || !portsValid)
            {
                LoggerUtil.LogInfoAndDebug("Failed manual user server query request: user's" +
                                           " multi-server query contained invalid hostname and/or port",
                    LogClassType);
            }

            return (hostsValid && portsValid);
        }

        /// <summary>
        ///     Checks whether the given hostname and port is valid.
        /// </summary>
        /// <param name="hostname">The hostname to check.</param>
        /// <param name="port">The port to check.</param>
        /// <returns><c>true</c> if both the host and port is valid, otherwise <c>false</c>if either are invalid.</returns>
        private bool HostsPortsValid(string hostname, string port)
        {
            bool portValid;
            var hostValid = (Uri.CheckHostName(hostname) != UriHostNameType.Unknown);
            int portNum;
            if (!int.TryParse(port, out portNum))
            {
                portValid = false;
            }
            else if (portNum > 65535)
            {
                portValid = false;
            }
            else
            {
                portValid = true;
            }

            if (!hostValid || !portValid)
            {
                LoggerUtil.LogInfoAndDebug(string.Format("Failed manual user server query request:" +
                                                         " user's single-server query contained invalid hostname and/or port, host: {0}, port: {1}, ",
                    hostname, port), LogClassType);
            }

            return (hostValid && portValid);
        }

        /// <summary>
        ///     Returns the invalid response details and logs the error for internal purposes.
        /// </summary>
        /// <param name="qrc">The query response code.</param>
        /// <returns>
        ///     A <see cref="PrivateQueryResponse" /> to the user that includes
        ///     an error message.
        /// </returns>
        private PrivateQueryResponse ReturnInvalidResponseDetails(QueryResponseCode qrc)
        {
            LoggerUtil.LogInfoAndDebug(qrc.LoggerMessage, LogClassType);
            return new PrivateQueryResponse {error = qrc.UserMessage};
        }
    }
}