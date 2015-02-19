using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo
{
	public class RawResponse
	{
		public byte[] Bytes { get; private set; }
		public string Hex { get; private set; }
		public string Base64 { get; private set; }
		public string RawString { get; private set; }

		public RawResponse(byte[] bytes)
		{
			Bytes = bytes;
			Hex = BitConverter.ToString(bytes);
			Base64 = Convert.ToBase64String(bytes);
			RawString = Encoding.UTF8.GetString(bytes);
		}

		public override string ToString()
		{
			return RawString;
		}
	}
}
