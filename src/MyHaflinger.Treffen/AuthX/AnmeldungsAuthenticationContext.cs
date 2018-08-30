using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyHaflinger.Treffen.AuthX
{
	public class AnmeldungsAuthenticationContext
	{
		private readonly string _jsonFilePath;

		public AnmeldungsAuthenticationContext(string jsonFilePath)
		{
			_jsonFilePath = jsonFilePath;
		}

		public LogonUser GetUser(string username, string password)
		{
			string fileContents = File.ReadAllText(_jsonFilePath);
			var definedAccounts = JsonConvert.DeserializeObject<List<LogonUser>>(fileContents);

			return definedAccounts.FirstOrDefault(a => a.Username == username && a.Password == password);
		}

		// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.1&tabs=aspnetcore2x
		public ClaimsIdentity CreateIdentity(string username, string role)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, username),
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.Role, role),
			};

			return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
