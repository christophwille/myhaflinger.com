//        public static void Main(string[] args)
//        {
//            // https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2
//            // https://github.com/NLog/NLog.Web/issues/211 (see for version dependencies)
//            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

//            BuildWebHost(args).Run();
//        }

//        public static IWebHost BuildWebHost(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>()
//                .UseNLog()
//                .Build();
//    }
// https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyHaflinger.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}