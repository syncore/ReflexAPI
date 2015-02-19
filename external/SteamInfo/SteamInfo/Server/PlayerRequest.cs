using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Server
{
	public class PlayerRequest : BaseServerRequest, IServerRequest
	{
		private readonly int _token;

		public PlayerRequest(int token)
		{
			_token = token;
		}

		public byte[] ToBytes()
		{
			/* https://developer.valvesoftware.com/wiki/Server_Queries
			 * https://developer.valvesoftware.com/wiki/Talk:Server_queries
			 * 
			 *		+------+-----+
			 *		|Header|Token|
			 *		+------+-----+
			 *
			 * Header - 1 byte
			 *		Always 0x55 (the character "U").
			 * 
			 * Token - int
			 *		The challenge number to use.
			 */

			var request = GetRequest();
			request.Add(0x55);
			request.AddRange(BitConverter.GetBytes(_token));

			return request.ToArray();
		}
	}
}
