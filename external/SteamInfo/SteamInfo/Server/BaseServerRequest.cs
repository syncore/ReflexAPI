using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Server
{
	public class BaseServerRequest
	{
		public List<byte> GetRequest()
		{
			return new List<byte>(BitConverter.GetBytes((int)PacketFormat.Simple));
		}
	}
}
