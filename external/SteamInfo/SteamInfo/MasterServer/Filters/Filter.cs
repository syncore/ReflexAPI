using System;
using System.Text;
using SteamInfo.Extensions;

namespace SteamInfo.MasterServer.Filters
{
	public class Filter
	{
		public Region? Region { get; set; }

		public Game? Game { get; set; }
		public bool? HasPlayers { get; set; }
		public bool? IsNotFull { get; set; }
		public bool? IsVacProtected { get; set; }
		public string Map { get; set; }

		public string ToFilterString()
		{
			var sb = new StringBuilder();

			if (Game.HasValue)
				sb.Append(Game.GetDescription());

			if (HasPlayers.HasValue)
				sb.Append(HasPlayers.Value ? @"\empty\1" : @"\noplayers\1");

			if (IsNotFull.HasValue && IsNotFull.Value)
				sb.Append(@"\full\1");

			if (IsVacProtected.HasValue && IsVacProtected.Value)
				sb.Append(@"\secure\1");
			
			sb.Append(Map);

			return sb.ToString();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			if (Region.HasValue)
				sb.Append(Region).Append(" - ");

			sb.Append(ToFilterString());

			return sb.ToString();
		}
	}
}
