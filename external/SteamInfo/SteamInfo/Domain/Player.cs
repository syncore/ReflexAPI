using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Domain
{
	public class Player
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long? SteamId { get; set; }
		public long QueryCount { get; set; }
		public DateTime QueriedAt { get; set; }
	}
}
