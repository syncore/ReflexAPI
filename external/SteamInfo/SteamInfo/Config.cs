using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo
{
	public static class Config
	{
		private static bool? _useCache;
		public static bool UseCache
		{
			get
			{
				if (!_useCache.HasValue)
				{
					_useCache = false;

					bool b;
					if (bool.TryParse(ConfigurationManager.AppSettings["UseCache"], out b))
						_useCache = b;
				}

				return _useCache.Value;
			}
		}
	}
}
