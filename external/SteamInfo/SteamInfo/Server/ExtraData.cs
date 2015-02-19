using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamInfo.Exceptions;
using SteamInfo.Extensions;

namespace SteamInfo.Server
{
	public class ExtraData
	{
		public int? Port { get; private set; }
		public ulong? UnsignedServerSteamId { get; private set; }
		public long? SignedServerSteamId { get; private set; }
		public int? SourceTvPort { get; private set; }
		public string SourceTvName { get; private set; }
		public string Keywords { get; private set; }
		public ulong? GameSteamId { get; private set; }

		public ExtraData(byte[] bytes)
		{
			Parse(bytes);
		}

		private void Parse(byte[] bytes)
		{
			var flag = bytes[0];
			bytes = bytes.RemoveFromStart(1);

			var hasPort = (flag & 0x80) > 0;
			var hasSteamId = (flag & 0x10) > 0;
			var hasSourceTv = (flag & 0x40) > 0;
			var hasKeywords = (flag & 0x20) > 0;
			var hasGameId = (flag & 0x01) > 0;

			if (hasPort)
			{
				Port = bytes.ReadInt16();
				bytes = bytes.RemoveFromStart(2);
			}

			if (hasSteamId)
			{
				UnsignedServerSteamId = bytes.ReadUInt64();
				SignedServerSteamId = bytes.ReadInt64();
				bytes = bytes.RemoveFromStart(8);
			}

			if (hasSourceTv)
			{
				SourceTvPort = bytes.ReadInt16();
				bytes = bytes.RemoveFromStart(2);

				SourceTvName = bytes.ReadStringUntilNullTerminator();
				bytes = bytes.RemoveFromStart(SourceTvName.Length + 1);
			}

			if (hasKeywords)
			{
				Keywords = bytes.ReadStringUntilNullTerminator();
				bytes = bytes.RemoveFromStart(Keywords.Length + 1);
			}

			if (hasGameId)
			{
				GameSteamId = bytes.ReadUInt64();
				bytes = bytes.RemoveFromStart(8);
			}

			if (bytes.Any())
				throw new ResponseLengthException();
		}
	}
}
