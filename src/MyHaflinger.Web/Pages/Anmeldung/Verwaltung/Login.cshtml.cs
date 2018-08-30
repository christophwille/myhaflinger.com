using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	public class LoginModel : PageModel
	{
		[BindProperty]
		[Required]
		public string Username { get; set; }
		[BindProperty]
		[Required]
		public string Password { get; set; }

		public async Task<IActionResult> OnPostAsync([FromServices]AnmeldungsAuthenticationFactory authFactory, [FromServices]RegistrationFlowAuditTrailService auditLog)
		{
			if (!ModelState.IsValid) return Page();

			var authContext = authFactory.CreateContext();
			var user = authContext.GetUser(Username, Password);

			string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

			if (user != null)
			{
				var claimsIdentity = authContext.CreateIdentity(user.Username, user.Role);

				var authProperties = new AuthenticationProperties
				{
					//AllowRefresh = <bool>,
					// Refreshing the authentication session should be allowed.

					//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
					// The time at which the authentication ticket expires. A 
					// value set here overrides the ExpireTimeSpan option of 
					// CookieAuthenticationOptions set with AddCookie.

					// IsPersistent = true,
					// Whether the authentication session is persisted across 
					// multiple requests. Required when setting the 
					// ExpireTimeSpan option of CookieAuthenticationOptions 
					// set with AddCookie. Also required when setting 
					// ExpiresUtc.

					//IssuedUtc = <DateTimeOffset>,
					// The time at which the authentication ticket was issued.

					//RedirectUri = <string>
					// The full path or absolute URI to be used as an http 
					// redirect response value.
				};

				auditLog.Trace($"SEC:S: User {user.Username} logged in from {ipAddress}");

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);
			}
			else
			{
				auditLog.Trace($"SEC:F: Invalid login atttempt for user {Username} from {ipAddress}");
			}

			return Page();
		}
	}
}