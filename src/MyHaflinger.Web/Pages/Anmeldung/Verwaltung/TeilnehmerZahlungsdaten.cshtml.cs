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

		public void OnGet([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			var ctx = dbFactory.CreateContext();
			Registration = ctx.GetRegisteredParticipant(id);

			PaymentAmount = Registration.IntPaymentReceivedAmount;
			PaymentDate = Registration.IntPaymentReceivedDate;
			Notes = Registration.IntPaymentNotes;
		}

		public IActionResult OnPost([FromServices]AnmeldungsDbFactory dbFactory, int id)
		{
			if (!ModelState.IsValid) return Page();

			var ctx = dbFactory.CreateContext();
			var reg = ctx.GetRegisteredParticipant(id);

			reg.IntPaymentReceivedAmount = PaymentAmount;
			reg.IntPaymentReceivedDate = PaymentDate;
			reg.IntPaymentNotes = Notes;

			string modMessage = $"{User.Identity.Name} hat um {DateTime.UtcNow} Zahlungsinfos verändert\r\n";
			reg.IntModificationLog = modMessage + reg.IntModificationLog;

			ctx.UpdateRegisteredParticipant(reg);
			return RedirectToPage("Teilnehmer");
		}
	}
}