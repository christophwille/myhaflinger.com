using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Treffen.Db;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[Authorize]
	[RequireHttps]
	public class TeilnehmerModel : PageModel
	{
		public List<Registration> Registrations { get; private set; }

		public async Task OnGetAsync([FromServices]AnmeldungsDbFactory dbFactory)
		{
			var ctx = await dbFactory.CreateContextAsync();
			Registrations = await ctx.GetRegisteredParticipantsAsync();
		}
	}
}