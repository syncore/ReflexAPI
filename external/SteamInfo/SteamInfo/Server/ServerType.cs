using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Server
{
	public enum ServerType
	{
									// Ascii
		Dedicated = 100,		// 'd'
		NonDedicated = 108,	// 'l'
		Hltv = 112				// 'p'
	}
}
