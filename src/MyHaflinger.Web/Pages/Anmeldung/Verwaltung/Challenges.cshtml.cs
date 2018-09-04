using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Treffen.Db;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[Authorize]
	[RequireHttps]
	public class ChallengesModel : PageModel
	{
		public List<EmailChallenge> Challenges { get; private set; }
		public string Scheme { get; private set; }
		public string Host { get; private set; }

		public async Task OnGetAsync([FromServices]AnmeldungsDbFactory dbFactory)
		{
			Challenges = await (await dbFactory.CreateContextAsync()).GetEmailChallengesAsync();

			Scheme = HttpContext.Request.Scheme;
			Host = HttpContext.Request.Host.Value;
		}
	}
}