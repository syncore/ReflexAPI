using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamInfo.Exceptions;
using SteamInfo.Extensions;

namespace SteamInfo.Server
{
	public class InfoResponse
	{
		public RawResponse RawResponse { get; private set; }

		public PacketFormat PacketFormat { get; private set; }
		public string Header { get; private set; }
		public int Protocol { get; private set; }
		public string Name { get; private set; }
		public string Map { get; private set; }
		public string Folder { get; private set; }
		public string Game { get; private set; }
		public int Id { get; private set; }
		public int Players { get; private set; }
		public int MaxPlayers { get; private set; }
		public int Bots { get; private set; }
		public ServerType ServerType { get; private set; }
		public Environment Environment { get; private set; }
		public bool RequiresPassword { get; private set; }
		public bool IsVacProtected { get; private set; }
		public string Version { get; private set; }
		public ExtraData ExtraData { get; private set; }

		public InfoResponse(byte[] bytes)
		{
			RawResponse = new RawResponse(bytes);
			Parse(bytes);
		}

		private void Parse(byte[] bytes)
		{
			/* https://developer.valvesoftware.com/wiki/Server_Queries
			 * https://developer.valvesoftware.com/wiki/Talk:Server_queries
			 * 
			 *		+------+--------+----+---+------+----+--+-------+-----------+----+-----------+-----------+----------+--------+-------+---+--+
			 *		|Header|Protocol|Name|Map|Folder|Game|ID|Players|Max Players|Bots|Server Type|Environment|Visibility|The Ship|Version|EDF|ED|
			 *		+------+--------+----+---+------+----+--+-------+-----------+----+-----------+-----------+----------+--------+-------+---+--+
			 *
			 * Header - 1 byte
			 *		Always 0x49 (the character "I").
			 * 
			 * Protocol - 1 byte
			 *		Protocol version used by the server.
			 *		
			 * Name - string
			 *		Name of the server.
			 *		
			 * Map - string
			 *		Map the server has currently loaded.
			 *		
			 * Folder - string
			 *		Name of the folder containing the game files.
			 *		
			 * Game - string
			 *		Full name of the game.
			 *		
			 * ID - Int16, 2 bytes
			 *		Steam Application ID of game.
			 *		https://developer.valvesoftware.com/wiki/Steam_Application_ID
			 *		
			 * Players - byte
			 *		Number of players on the server.
			 *		
			 * Max Players - byte
			 *		Maximum number of players the server reports it can hold.
			 *		
			 * Bots - byte
			 *		Number of bots on the server.
			 *		
			 * Server Type - byte
			 *		Indicates the type of server:
			 *			'd' for a dedicated server
			 *			'l' for a non-dedicated server
			 *			'p' for a SourceTV server
			 *			
			 * Environment - byte
			 *		Indicates the operating system of the server:
			 *			'l' for Linux
			 *			'w' for Windows
			 *			
			 * Visibility - byte
			 *		Indicates whether the server requires a password:
			 *			0 for public
			 *			1 for private
			 *			
			 * VAC - byte
			 *		Specifies whether the server uses VAC:
			 *			0 for unsecured
			 *			1 for secured
			 *			
			 * The Ship - IGNORED FOR NOW
			 * 
			 * Version - string
			 *		Version of the game installed on the server.
			 *		
			 * Extra Data Flag (EDF) - byte
			 *		If present, this specifies which additional data fields will be included.
			 *		
			 * Extra Data (ED)
			 *		If (ExtraData & 0x80) > 0
			 *			Port - short
			 *				The server's game port number.
			 *		If (ExtraData & 0x10) > 0
			 *			SteamID - long
			 *				Server's SteamID.
			 *		If (ExtraData & 0x40) > 0
			 *			Port - short
			 *				Spectator port number for SourceTV.
			 *			Name - string
			 *			Name of the spectator server for SourceTV.
			 *		If (ExtraData & 0x20) > 0
			 *			Keywords - string
			 *			Tags that describe the game according to the server.
			 *		If (ExtraData & 0x01) > 0
			 *			GameID - long
			 *			The server's 64-bit GameID.
			 */

			const string expectedHeader = "I";

			PacketFormat = (PacketFormat) bytes.ReadInt();
			bytes = bytes.RemoveFromStart(4);

			if (PacketFormat != PacketFormat.Simple)
				throw new NotImplementedException("Not ready to handle multi-packet responses yet."); // TODO
			
			Header = bytes.ReadString(1);
			bytes = bytes.RemoveFromStart(1);

			if (Header != expectedHeader)
				throw new ResponseHeaderException();

			Protocol = bytes[0];
			bytes = bytes.RemoveFromStart(1);

			Name = bytes.ReadStringUntilNullTerminator();
			bytes = bytes.RemoveFromStart(Name.Length + 1);

			Map = bytes.ReadStringUntilNullTerminator();
			bytes = bytes.RemoveFromStart(Map.Length + 1);

			Folder = bytes.ReadStringUntilNullTerminator();
			bytes = bytes.RemoveFromStart(Folder.Length + 1);

			Game = bytes.ReadStringUntilNullTerminator();
			bytes = bytes.RemoveFromStart(Game.Length + 1);

			Id = bytes.ReadInt16();
			bytes = bytes.RemoveFromStart(2);

			if (Id >= 2400 && Id <= 2412)
				throw new HolyShitException("You've found a server running 'The Ship'! This is ignored.");

			Players = bytes[0];
			bytes = bytes.RemoveFromStart(1);

			MaxPlayers = bytes[0];
			bytes = bytes.RemoveFromStart(1);

			Bots = bytes[0];
			bytes = bytes.RemoveFromStart(1);

			ServerType = (ServerType) bytes[0];
			bytes = bytes.RemoveFromStart(1);

			Environment = (Environment) bytes[0];
			bytes = bytes.RemoveFromStart(1);

			RequiresPassword = bytes.ReadBoolean();
			bytes = bytes.RemoveFromStart(1);

			IsVacProtected = bytes.ReadBoolean();
			bytes = bytes.RemoveFromStart(1);

			Version = bytes.ReadStringUntilNullTerminator();
			bytes = bytes.RemoveFromStart(Version.Length + 1);

			ExtraData = new ExtraData(bytes);
		}

		public override string ToString()
		{
			return string.Format("{0} | {1} | {2}/{3}", Name, Map, Players, MaxPlayers);
		}
	}
}
