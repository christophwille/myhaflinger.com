using System;
using System.Collections.Generic;
using System.Linq;
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
	public class TeilnehmerDetailsModel : PageModel
	{
		public Registration Registration { get; private set; }

		public async Task OnGetAsync([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			var ctx = await dbFactory.CreateContextAsync();
			Registration = await ctx.GetRegisteredParticipantAsync(id);
		}
	}
}