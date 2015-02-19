using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Domain
{
	public class CacheContextInitializer : IDatabaseInitializer<CacheContext>
	{
		public void InitializeDatabase(CacheContext context)
		{
			const string sql = @"CREATE TABLE IF NOT EXISTS [Player] (
				[Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
				[Name] TEXT NOT NULL,
				[SteamId] INTEGER,
				[QueryCount] INTEGER NOT NULL,
				[QueriedAt] DATETIME NOT NULL
			)";

			context.Database.ExecuteSqlCommand(sql);
		}
	}
}
