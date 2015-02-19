using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Server
{
	public class InfoRequest : BaseServerRequest, IServerRequest
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
			 *		Always 0x54 (the character "T").
			 * 
			 * Payload - string
			 *		Always "Source Engine Query"
			 */

			var request = GetRequest();
			request.Add(0x54);
			request.AddRange(Encoding.UTF8.GetBytes("Source Engine Query"));
			request.Add(0x00);


			return request.ToArray();
		}
	}
}
