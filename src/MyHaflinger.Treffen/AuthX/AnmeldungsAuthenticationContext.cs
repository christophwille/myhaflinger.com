using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using MyHaflinger.Common;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyHaflinger.Treffen.AuthX
{
	public class AnmeldungsAuthenticationContext
	{
		private readonly string _jsonFilePath;

		public AnmeldungsAuthenticationContext(string jsonFilePath)
		{
			_jsonFilePath = jsonFilePath;
		}

		public async Task<LogonUser> GetUserAsync(string username, string password)
		{
			using (var stream = File.OpenRead(_jsonFilePath))
			{
				var options = new JsonSerializerOptions
				{
					AllowTrailingCommas = true,
					PropertyNameCaseInsensitive = true
				};

				var definedAccounts = await JsonSerializer.DeserializeAsync<List<LogonUser>>(stream, options);
				stream.Close();

				var account = definedAccounts.FirstOrDefault(a => a.Username == username);
				if (account != null)
				{
					if (PBKDF2.ValidatePassword(password, account.Password)) return account;
				}
			}
			return null;
		}

		public const string AuthenticationScheme = "Cookies";

		// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.1&tabs=aspnetcore2x
		public ClaimsIdentity CreateIdentity(string username, string role)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, username),
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.Role, role),
			};

			return new ClaimsIdentity(claims, AuthenticationScheme);
		}
	}
}
