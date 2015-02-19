using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SteamInfo.Extensions
{
	internal static class ArrayExtensions
	{
		internal static int ReadInt(this byte[] bytes)
		{
			return BitConverter.ToInt32(bytes, 0);
		}

		internal static short ReadInt16(this byte[] bytes)
		{
			return BitConverter.ToInt16(bytes, 0);
		}

		internal static ulong ReadUInt64(this byte[] bytes)
		{
			return BitConverter.ToUInt64(bytes, 0);
		}

		internal static long ReadInt64(this byte[] bytes)
		{
			return BitConverter.ToInt64(bytes, 0);
		}

		internal static float ReadSingle(this byte[] bytes)
		{
			return BitConverter.ToSingle(bytes, 0);
		}

		internal static string ReadHex(this byte[] bytes)
		{
			return BitConverter.ToString(bytes, 0, 1);
		}

		internal static bool ReadBoolean(this byte[] bytes)
		{
			return BitConverter.ToBoolean(bytes, 0);
		}

		internal static string ReadString(this byte[] bytes, int length)
		{
			var subArray = bytes.Take(length).ToArray();
			return Encoding.UTF8.GetString(subArray);
		}

		internal static string ReadStringUntilNullTerminator(this byte[] bytes)
		{
			const byte nullTerminator = 0x00;
			var subArray = bytes.TakeWhile(b => b != nullTerminator).ToArray();
			return Encoding.UTF8.GetString(subArray);
		}

		public static string ReadStringUntilNullTerminator(this byte[] bytes, out int length)
		{
			const byte nullTerminator = 0x00;
			var subArray = bytes.TakeWhile(b => b != nullTerminator).ToArray();
			length = subArray.Length;
			return Encoding.UTF8.GetString(subArray);
		}

		internal static byte[] RemoveFromStart(this byte[] bytes, int count)
		{
			return bytes
				.Skip(count)
				.ToArray();
		}

		internal static byte[] RemoveFromEnd(this byte[] bytes, int count)
		{
			bytes = bytes
				.Reverse()
				.ToArray();

			bytes = bytes.RemoveFromStart(count);

			return bytes
				.Reverse()
				.ToArray();
		}
	}
}
