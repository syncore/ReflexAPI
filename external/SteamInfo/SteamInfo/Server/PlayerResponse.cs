using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamInfo.Exceptions;
using SteamInfo.Extensions;

namespace SteamInfo.Server
{
	public class PlayerResponse
	{
		public RawResponse RawResponse { get; private set; }

		public PacketFormat PacketFormat { get; private set; }
		public string Header { get; private set; }
		public List<Player> Players { get; private set; }

		public PlayerResponse(byte[] bytes)
		{
			RawResponse = new RawResponse(bytes);
			Parse(bytes);
		}

		private void Parse(byte[] bytes)
		{
			/* https://developer.valvesoftware.com/wiki/Server_Queries
			 * https://developer.valvesoftware.com/wiki/Talk:Server_queries
			 * 
			 *		       +---------- Players -----------+
			 *		       v                              v
			 *		+------+--------+--------+---+--------+
			 *		|Header|Player 1|Player 2|...|Player n|
			 *		+------+--------+--------+---+--------+
			 *
			 * Header - byte
			 *		Always 0x44 (the character "D").
			 * 
			 * Players
			 *		Represents a list of players.
			 *		
			 *		Player
			 *			+-----+----+-----+--------+
			 *			|Index|Name|Score|Duration|
			 *			+-----+----+-----+--------+
			 *			
			 *			Index - byte
			 *				Supposed to be the index of player starting from 0. I don't actually
			 *				think this is used.
			 *				
			 *			Name - string
			 *				Name of the player.
			 *				
			 *			Score - int
			 *				Player's score (usually "frags" or "kills").
			 *				
			 *			Duration - float
			 *				Time (in seconds) player has been connected to the server.			 *			
			 */

			const string expectedHeader = "D";

			PacketFormat = (PacketFormat) bytes.ReadInt();
			bytes = bytes.RemoveFromStart(4);

			if (PacketFormat != PacketFormat.Simple)
				throw new NotImplementedException("Not ready to handle multi-packet responses yet."); // TODO
			
			Header = bytes.ReadString(1);
			bytes = bytes.RemoveFromStart(1);

			if (Header != expectedHeader)
				throw new ResponseHeaderException();

			var playerCount = bytes[0];
			bytes = bytes.RemoveFromStart(1);

			Players = new List<Player>();

			for (var i = 0; i < playerCount; i++)
			{
				var player = new Player();

				player.Index = bytes[0];
				bytes = bytes.RemoveFromStart(1);

				if (player.Index != 0)
					throw new HolyShitException("Player index is used!");

				int length;
				player.Name = bytes.ReadStringUntilNullTerminator(out length);

				bytes = bytes.RemoveFromStart(length + 1);

				player.Score = bytes.ReadInt();
				bytes = bytes.RemoveFromStart(4);

				player.Duration = bytes.ReadSingle();
				bytes = bytes.RemoveFromStart(4);

				Players.Add(player);
			}

			if (bytes.Any())
				throw new ResponseLengthException();
		}

		public override string ToString()
		{
			return string.Format("{0} player(s) | {1}", Players.Count, string.Join(" | ", Players.Select(p => p.Name)));
		}
	}
}
