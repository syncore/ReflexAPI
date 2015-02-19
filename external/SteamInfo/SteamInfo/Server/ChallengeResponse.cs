using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SteamInfo.Exceptions;
using SteamInfo.Extensions;

namespace SteamInfo.Server
{
	public class ChallengeResponse
	{
		public RawResponse RawResponse { get; private set; }

		public PacketFormat PacketFormat { get; private set; }
		public string Header { get; private set; }
		public int Token { get; private set; }

		public ChallengeResponse(byte[] bytes)
		{
			RawResponse = new RawResponse(bytes);
			Parse(bytes);
		}

		private void Parse(byte[] bytes)
		{
			/* https://developer.valvesoftware.com/wiki/Server_Queries
			 * https://developer.valvesoftware.com/wiki/Talk:Server_queries
			 * 
			 *		+------+-----+
			 *		|Header|Token|
			 *		+------+-----+
			 *
			 * Header - 1 byte
			 *		Always 0x41 (the character "A").
			 * 
			 * Token - int
			 *		The challenge number to use.
			 */

			const string expectedHeader = "A";

			PacketFormat = (PacketFormat) bytes.ReadInt();
			bytes = bytes.RemoveFromStart(4);

			if (PacketFormat != PacketFormat.Simple)
				throw new NotImplementedException("Not ready to handle multi-packet responses yet."); // TODO
			
			Header = bytes.ReadString(1);
			bytes = bytes.RemoveFromStart(1);

			if (Header != expectedHeader)
				throw new ResponseHeaderException();

			Token = bytes.ReadInt();
			bytes = bytes.RemoveFromStart(4);

			if (bytes.Any())
				throw new ResponseLengthException();
		}

		public override string ToString()
		{
			return Token.ToString();
		}
	}
}
