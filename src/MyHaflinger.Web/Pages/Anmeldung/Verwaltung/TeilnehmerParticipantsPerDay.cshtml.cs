using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Treffen.AuthX;
using MyHaflinger.Treffen.Db;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[Authorize(Roles = AnmeldungRoles.Kassier)]
	[RequireHttps]
	public class TeilnehmerParticipantsPerDayModel : PageModel
	{
		[BindProperty]
		[Required(ErrorMessage = "Teilnehmer Freitag muß eingegeben werden")]
		[Range(0, 10, ErrorMessage = "Wert muß größer/gleich 0 sein (und kleiner 10)")]
		public int ParticipantsFriday { get; set; }

		[BindProperty]
		[Required(ErrorMessage = "Teilnehmer Sa/So muß eingegeben werden")]
		[Range(0, 10, ErrorMessage = "Wert muß größer/gleich 0 sein (und kleiner 10)")]
		public int ParticipantsSatSun { get; set; }

		public Registration Registration { get; private set; }

		public async Task OnGetAsync([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			var ctx = await dbFactory.CreateContextAsync();
			Registration = await ctx.GetRegisteredParticipantAsync(id);

			ParticipantsFriday = Registration.PParticipantsFriday;
			ParticipantsSatSun = Registration.PParticipantsSatSun;
		}

		public async Task<IActionResult> OnPostAsync([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			if (!ModelState.IsValid) return Page();

			var ctx = await dbFactory.CreateContextAsync();
			var reg = await ctx.GetRegisteredParticipantAsync(id);

			reg.PParticipantsFriday = ParticipantsFriday;
			reg.PParticipantsSatSun = ParticipantsSatSun;

			string modMessage = $"{User.Identity.Name} hat um {DateTime.UtcNow} Anzahl Personen Fr/Sa/So verändert\r\n";
			reg.IntModificationLog = modMessage + reg.IntModificationLog;

			await ctx.UpdateRegisteredParticipantAsync(reg);
			return RedirectToPage("Teilnehmer");
		}
	}
}