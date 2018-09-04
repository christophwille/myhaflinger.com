using System.Threading.Tasks;
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

		public async Task<AnmeldungsDbContext> CreateContextAsync()
		{
			var context = new AnmeldungsDbContext(_dbDatabaseFilePath);
			await context.CreateTablesAsync();
			return context;
		}
	}
}