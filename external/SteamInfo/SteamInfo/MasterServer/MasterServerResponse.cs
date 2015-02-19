using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SteamInfo.Exceptions;
using SteamInfo.Extensions;

namespace SteamInfo.MasterServer
{
	public class MasterServerResponse
	{
		private string _defaultRoute = "0.0.0.0:0";

		public RawResponse RawResponse { get; private set; }

		public List<IPEndPoint> Servers { get; private set; }
		public bool IsComplete { get; private set; }

		public MasterServerResponse(byte[] bytes)
		{
			RawResponse = new RawResponse(bytes);
			Servers = Parse(bytes);
		}

		private List<IPEndPoint> Parse(byte[] bytes)
		{
			/* https://developer.valvesoftware.com/wiki/Master_Server_Query_Protocol
			 * https://developer.valvesoftware.com/wiki/Talk:Master_Server_Query_Protocol
			 * 
			 *		       +--------- Servers ------------+
			 *		       v                              v
			 *		+------+--------+--------+---+--------+
			 *		|Header|Server 1|Server 2|...|Server n|
			 *		+------+--------+--------+---+--------+
			 *		Made with http://www.asciiflow.com
			 *
			 * Header - 6 bytes
			 *		Always { 0xFF, 0xFF, 0xFF, 0xFF, 0x66, 0x0A }.
			 * 
			 * Servers
			 *		Represents a list of servers that match the region and filter provided (if any).
			 *		The list may be split over multiple responses. 
			 *		
			 *		Multiple requests may have to be sent to get a full list of servers back over potentially
			 *		multiple responses. The last server returned should be used as the start server for the 
			 *		next request. Ports should be left open over multiple requests or else the list will be
			 *		started from the beginning again. The end of the list is signalled with "0.0.0.0:0".
			 * 	
			 *		Server - 6 bytes
			 *			The first 4 bytes are IP address octets. The last two bytes are an Int16 port number.
			 */

			const int headerLength = 6;
			var expectedHeader = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x66, 0x0A };
			var header = bytes.Take(headerLength).ToArray();

			if (!header.SequenceEqual(expectedHeader))
				throw new ResponseHeaderException();
			
			bytes = bytes.RemoveFromStart(headerLength);

			const int ipEndPointLength = 6;

			var servers = new List<IPEndPoint>();

			while (bytes.Length > 0)
			{
				var server = bytes.Take(ipEndPointLength).ToIPEndPoint();
				servers.Add(server);
				bytes = bytes.RemoveFromStart(ipEndPointLength);
			}

			if (servers.Last().ToString() == _defaultRoute)
			{
				servers.RemoveAt(servers.Count - 1);
				IsComplete = true;
			}

			return servers;
		}
	}
}
