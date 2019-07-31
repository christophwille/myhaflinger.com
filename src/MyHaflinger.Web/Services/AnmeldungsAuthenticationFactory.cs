using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MyHaflinger.Treffen.AuthX;

namespace MyHaflinger.Web.Services
{
	public class AnmeldungsAuthenticationFactory
	{
		private readonly string _jsonFilePath;
		public AnmeldungsAuthenticationFactory(IWebHostEnvironment env)
		{
			var webRoot = env.ContentRootPath;
			_jsonFilePath = System.IO.Path.Combine(webRoot, "Data", "AnmeldungAccounts.json");
		}

		public AnmeldungsAuthenticationContext CreateContext()
		{
			return new AnmeldungsAuthenticationContext(_jsonFilePath);
		}
	}
}
