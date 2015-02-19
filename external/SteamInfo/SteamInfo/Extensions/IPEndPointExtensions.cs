using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Extensions
{
	public static class IPEndPointExtensions
	{
		public static long? GetPing(this IPEndPoint server)
		{
			const int timeOutInMilliseconds = 100; // This seems to be buggy as shit :/
			var bytesToSend = new byte[32];

			long? time = null;

			var ping = new Ping();

			var reply = ping.Send(server.Address, timeOutInMilliseconds, bytesToSend);

			if (reply != null && reply.Status == IPStatus.Success)
				time = reply.RoundtripTime;

			return time;
		}
	}
}
