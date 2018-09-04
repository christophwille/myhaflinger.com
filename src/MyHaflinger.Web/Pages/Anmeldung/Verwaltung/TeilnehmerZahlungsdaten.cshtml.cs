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
	public class TeilnehmerZahlungsdatenModel : PageModel
	{
		[BindProperty]
		[Required(ErrorMessage = "Zahlungsbetrag muß angegeben sein")]
		[Range(0, 1000, ErrorMessage = "0 bis 1000 erlaubt")]
		public double PaymentAmount { get; set; }
		[BindProperty]
		public string PaymentDate { get; set; }
		[BindProperty]
		public string Notes { get; set; }


		public Registration Registration { get; private set; }

		public async Task OnGetAsync([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			var ctx = await dbFactory.CreateContextAsync();
			Registration = await ctx.GetRegisteredParticipantAsync(id);

			PaymentAmount = Registration.IntPaymentReceivedAmount;
			PaymentDate = Registration.IntPaymentReceivedDate;
			Notes = Registration.IntPaymentNotes;
		}

		public async Task<IActionResult> OnPostAsync([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			if (!ModelState.IsValid) return Page();

			var ctx = await dbFactory.CreateContextAsync();
			var reg = await ctx.GetRegisteredParticipantAsync(id);

			reg.IntPaymentReceivedAmount = PaymentAmount;
			reg.IntPaymentReceivedDate = PaymentDate;
			reg.IntPaymentNotes = Notes;

			string modMessage = $"{User.Identity.Name} hat um {DateTime.UtcNow} Zahlungsinfos verändert\r\n";
			reg.IntModificationLog = modMessage + reg.IntModificationLog;

			await ctx.UpdateRegisteredParticipantAsync(reg);
			return RedirectToPage("Teilnehmer");
		}
	}
}