using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SteamInfo.MasterServer.Filters;

namespace SteamInfo.MasterServer
{
	public class MasterServerRequest : IServerRequest
	{
		public byte RequestType { get; private set; }
		public Region Region { get; private set; }
		public IPEndPoint StartServer { get; private set; }
		public Filter Filter { get; set; }

		public MasterServerRequest(Filter filter) : this("0.0.0.0", 0, filter)
		{
		}

		public MasterServerRequest(string ipAddress, int port, Filter filter) : this(new IPEndPoint(IPAddress.Parse(ipAddress), port), filter)
		{
		}

		public MasterServerRequest(IPEndPoint startServer, Filter filter)
		{
			if (startServer == null)
				throw new ArgumentNullException("startServer", "Starting server can not be null.");

			RequestType = 0x31;
			StartServer = startServer;
			Filter = filter ?? new Filter();
			Region = Filter.Region ?? Region.RestOfTheWorld;
		}

		public byte[] ToBytes()
		{
			/* https://developer.valvesoftware.com/wiki/Master_Server_Query_Protocol
			 * https://developer.valvesoftware.com/wiki/Talk:Master_Server_Query_Protocol
			 * 
			 *		+------------+------+------------+------+
			 *		|Request Type|Region|Start server|Filter|
			 *		+------------+------+------------+------+
			 *
			 * Request Type - byte
			 *		Always 0x31 (the character "1").
			 * 
			 * Region - byte
			 *		The region of the world that you wish to find servers in.
			 *		
			 * Start server - string
			 *		Multiple requests may have to be sent to get a full list of servers back over potentially
			 *		multiple responses. The first request should be "0.0.0.0:0" and subsequent requests will be
			 *		the last server returned in a response. Ports should be left open over multiple requests
			 *		or else the list will be started from the beginning again.
			 *		
			 * Filter - string
			 *		Allows you to restrict the results to servers running a certain game, map, etc. Insert "\" in
			 *		between filter parameters.
			 */

			const int typeLength = 1;
			const int regionLength = 1;
			const int nullTerminatorLength = 1;
			const byte nullTerminator = 0x00;

			var utf = new UTF8Encoding();

			var ipAddress = utf.GetBytes(StartServer.ToString());
			var filter = utf.GetBytes(Filter.ToFilterString());

			var totalLength = typeLength
				+ regionLength
				+ ipAddress.Length
				+ nullTerminatorLength
				+ filter.Length
				+ nullTerminatorLength;

			var bytes = new byte[totalLength];
			var i = 0;

			bytes[i] = RequestType;
			i++;

			bytes[i] = (byte) Region;
			i++;

			ipAddress.CopyTo(bytes, i);
			i += ipAddress.Length;

			bytes[i] = nullTerminator;
			i++;

			if (filter.Any())
				filter.CopyTo(bytes, i);
			else
				bytes[i] = nullTerminator;

			return bytes;
		}
	}
}
