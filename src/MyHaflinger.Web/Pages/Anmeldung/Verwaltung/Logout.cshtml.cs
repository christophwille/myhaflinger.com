using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Common;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[RequireHttps]
	public class LogoutModel : PageModel
	{
		public async Task<IActionResult> OnGet()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToPage("/Index");
		}
	}
}