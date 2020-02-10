using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Common;
using MyHaflinger.Common.Services;
using MyHaflinger.Treffen;
using MyHaflinger.Treffen.Services;
using MyHaflinger.Web.Models;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung
{
	[RequireHttps]
	public class FormularModel : PageModel
	{
		private readonly AnmeldungsDbFactory _dbFactory;
		private readonly RegistrationFlowAuditTrailService _auditLog;
		private readonly AppOptions _ao;

		public FormActionEnum NextFormAction { get; set; }
		public string EmailAddress { get; set; }

		[BindProperty]
		[Required(ErrorMessage = "Name muß eingegeben werden")]
		public string Name { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Straße muß eingegeben werden")]
		public string Street { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Stadt muß eingegeben werden")]
		public string City { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Postleitzahl muß eingegeben werden")]
		public string Zip { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Land muß eingegeben werden")]
		public string Country { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Kennzeichen muß eingegeben werden")]
		public string LicensePlate { get; set; }
		[BindProperty]
		public string Phone { get; set; }
		[BindProperty]
		public string Notes { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Teilnehmer Freitag muß eingegeben werden")]
		[Range(0, 10, ErrorMessage = "Wert muß größer/gleich 0 sein (und kleiner 10)")]
		public int ParticipantsFriday { get; set; }
		[BindProperty]
		[Required(ErrorMessage = "Teilnehmer Sa/So muß eingegeben werden")]
		[Range(0, 10,ErrorMessage = "Wert muß größer/gleich 0 sein (und kleiner 10)")]
		public int ParticipantsSatSun { get; set; }

		[BindProperty]
		public bool Teilnahmebedingungen { get; set; }

		public string TotalPriceOnCompletion { get; set; }


		public FormularModel(AnmeldungsDbFactory dbFactory, RegistrationFlowAuditTrailService auditLog, AppOptions ao)
		{
			_dbFactory = dbFactory;
			_auditLog = auditLog;
			_ao = ao;

			NextFormAction = FormActionEnum.ShowFormInitError;
		}

		private async Task<string> RetrieveEmailAddressForToken(string token)
		{
			var dbCtx = await _dbFactory.CreateContextAsync();
			return await dbCtx.GetEmailForChallengeTokenAsync(token);
		}

		// URL sample: http://myhaflinger.com/Anmeldung/Formular?token=2a75f00f-9e42-4698-8f59-60a9ff56cd0e
		public async Task<IActionResult> OnGetAsync(string token)
		{
			if (!_ao.RegistrationOpen) return RedirectToPage("/Index");

			string emailAddress = await RetrieveEmailAddressForToken(token);

			if (String.IsNullOrWhiteSpace(emailAddress))
			{
				_auditLog.Trace($"REG:S:F:ERR: Invalid token {token}");
				NextFormAction = FormActionEnum.ShowFormInitError;
			}
			else
			{
				EmailAddress = emailAddress;
				ParticipantsFriday = 0;
				ParticipantsSatSun = 1;
				NextFormAction = FormActionEnum.ShowForm;
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync([FromServices]ISmtpMailService smtpMailService, [FromServices]ITemplateRenderingService templateRenderer, string token)
		{
			if (!_ao.RegistrationOpen) return RedirectToPage("/Index");

			EmailAddress = await RetrieveEmailAddressForToken(token);

			// This is over-the-top security in our simple reg case, just to get rid of the overly curious playing with the query string
			if (String.IsNullOrWhiteSpace(EmailAddress))
			{
				_auditLog.Trace($"REG:S:F:ERR: Invalid token {token}");
				NextFormAction = FormActionEnum.ShowFormInitError;
				return Page();
			}

			if (!ModelState.IsValid || !Teilnahmebedingungen)
			{
				NextFormAction = FormActionEnum.ShowForm;

				if (!Teilnahmebedingungen) ModelState.AddModelError("Teilnahmebedingungen", "Teilnahmebedingungen müssen akzeptiert werden");
				return Page();
			}

			// Save to database
			var reg = new MyHaflinger.Treffen.Db.Registration
			{
				Name = Name,
				Street = Street,
				Zip = Zip,
				City = City,
				Country = Country,
				NumberPlate = LicensePlate,
				Phone = Phone,
				Notes = Notes,
				PParticipantsFriday = ParticipantsFriday,
				PParticipantsSatSun = ParticipantsSatSun,
				EmailAddress = EmailAddress,
				RegisteredAt = DateTime.UtcNow
			};

			var dbCtx = await _dbFactory.CreateContextAsync();
			reg = await dbCtx.RegisterParticipantAsync(reg);

			string reg4Logging = JsonSerializer.Serialize(reg);
			_auditLog.Trace($"REG:S:F: Teilnehmer {reg.Name} was registered properly");
			_auditLog.Trace($"REG:S:F:D: Teilnehmer {reg.Name} data: {reg4Logging}");

			// TODO: Mark token as used

			// Send email to registrant
			var mm = new AnmeldungMailService(smtpMailService, templateRenderer, _ao.MailFromAddress);
			bool sentOkToRegistrant = await mm.SendRegCompleteMailToParticipant(reg);

			// TODO: Handle mail sending erros

			// Send email to registration desk
			bool sentOkToOrga = await mm.SendNewRegInfoToRegDesk(AnmeldungSettings.RegDeskEmailAddress, reg);

			TotalPriceOnCompletion = reg.TotalPrice.ToString();
			NextFormAction = FormActionEnum.ShowCompletionInfo;

			return Page();
		}
	}
}