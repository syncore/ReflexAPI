using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace SteamInfo.Extensions
{
	internal static class EnumerableExtensions
	{
		internal static IPEndPoint ToIPEndPoint(this IEnumerable<byte> bytes)
		{
			var array = bytes.ToArray();
			Debug.Assert(array.Length == 6);

			var ipAddressArray = array
				.Take(4)
				.ToArray();

			var portArray = array
				.Skip(4)
				.Take(2)
				.ToArray();

			var ipAddress = new IPAddress(ipAddressArray);
			var port = (portArray[0] << 8) | portArray[1];
			
			return new IPEndPoint(ipAddress, port);
		}
	}
}
