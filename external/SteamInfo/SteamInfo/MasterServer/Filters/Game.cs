using System.ComponentModel;

namespace SteamInfo.MasterServer.Filters
{
	public enum Game
	{
		[Description(@"\gamedir\tf")]
		TeamFortress2,

		[Description(@"\gamedir\cstrike")]
		CounterStrikeSource,

        [Description(@"\appid\328070")]
        Reflex
	}
}
