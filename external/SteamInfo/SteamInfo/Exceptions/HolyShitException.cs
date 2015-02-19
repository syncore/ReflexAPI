using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Exceptions
{
	public class HolyShitException : Exception
	{
		public new string Message { get; private set; }

		public HolyShitException(string message)
		{
			Message = message;
		}
	}
}
