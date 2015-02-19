using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Server
{
	public class ChallengeRequest : BaseServerRequest, IServerRequest
	{
		public byte[] ToBytes()
		{
			/* https://developer.valvesoftware.com/wiki/Server_Queries
			 * https://developer.valvesoftware.com/wiki/Talk:Server_queries
			 * 
			 *		+------+-------+
			 *		|Header|Payload|
			 *		+------+-------+
			 *
			 * Header - 1 byte
			 *		Always 0x55 (the character "U").
			 * 
			 * Payload - int
			 *		Always -1
			 */

			var request = GetRequest();
			request.Add(0x55);
			request.AddRange(BitConverter.GetBytes(-1));

			return request.ToArray();
		}
	}
}
