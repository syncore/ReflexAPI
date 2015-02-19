using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.MasterServer.Filters
{
	public static class Maps
	{
		public static class Payload
		{
			public static string All = GoldRush + BadWaterBasin + Frontier + Hightower;

			public static string GoldRush = @"\map\pl_goldrush";
			public static string BadWaterBasin = @"\map\pl_badwater";
			public static string Frontier = @"\map\pl_frontier_final";
			public static string Hightower = @"\map\plr_hightower";
		}

		public static class SpecialDelivery
		{
			public static string Doomsday = @"\map\sd_doomsday";
		}

		public static class CaptureTheFlag
		{
			public static string Turbine = @"\map\ctf_turbine";
			public static string DoubleCross = @"\map\ctf_doublecross";
		}
	}
}
