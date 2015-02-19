using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamInfo.Domain
{
	public class CacheContext : DbContext
	{
		public DbSet<Player> Players { get; set; }

		public CacheContext()
		{
			Database.SetInitializer(new CacheContextInitializer());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}
