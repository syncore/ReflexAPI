using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Server
{
	public class Player
	{
		public int Index { get; set; }
		public string Name { get; set; }
		public int Score { get; set; }
		public float Duration { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
