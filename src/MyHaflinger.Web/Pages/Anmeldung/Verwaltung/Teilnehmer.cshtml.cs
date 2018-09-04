using System;
using System.Collections.Generic;
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

		public void OnGet([FromServices]AnmeldungsDbFactory dbFactory)
		{
			var ctx = dbFactory.CreateContext();
			Registrations = ctx.GetRegisteredParticipants();
		}
	}
}