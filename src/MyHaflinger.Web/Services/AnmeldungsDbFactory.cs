using Microsoft.AspNetCore.Hosting;
using MyHaflinger.Treffen.Db;

namespace MyHaflinger.Web.Services
{
	public class AnmeldungsDbFactory
	{
		private readonly string _dbDatabaseFilePath;
		public AnmeldungsDbFactory(IHostingEnvironment env)
		{
			var webRoot = env.ContentRootPath;
			_dbDatabaseFilePath = System.IO.Path.Combine(webRoot, "Data", "anmeldungen.db");
		}

		public AnmeldungsDbContext CreateContext()
		{
			return new AnmeldungsDbContext(_dbDatabaseFilePath);
		}
	}
}