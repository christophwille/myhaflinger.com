﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MyHaflinger.Common.Services;
using MyHaflinger.Treffen.Services;
using MyHaflinger.Web.Models;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung
{
	public class StartModel : PageModel
	{
		private AnmeldungsDbFactory DbFactory { get; }
		public AppOptions _ao { get; }

		public StartModel(AnmeldungsDbFactory dbFactory, AppOptions ao)
		{
			DbFactory = dbFactory;
			_ao = ao;
		}

		[BindProperty]
		[RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")]
		[Required]
		public string EmailAddress { get; set; }

		public async Task<IActionResult> OnPostAsync([FromServices]ISmtpMailService smtpMailService, [FromServices]RegistrationFlowAuditTrailService auditLog)
		{
			if (!ModelState.IsValid) return Page();

			string guid = Guid.NewGuid().ToString();

			var ctx = DbFactory.CreateContext();
			ctx.RegisterEmailChallenge(EmailAddress, guid, DateTime.UtcNow);

			auditLog.Trace($"REG:S:CH: Email {EmailAddress} registered for challenge");

			string scheme = HttpContext.Request.Scheme;
			string host = HttpContext.Request.Host.Value;
			string formularUrl = $"{scheme}://{host}/Anmeldung/Formular?token={guid}";

			var mm = new AnmeldungMailService(smtpMailService, _ao.MailFromAddress);
			bool mailSentOk = await mm.SendStep1Mail(formularUrl, EmailAddress);

			if (!mailSentOk)
			{
				ModelState.AddModelError("mailfail", $"Mail an {EmailAddress} konnte nicht gesendet werden. Vertippt?");
				return Page();
			}

			return RedirectToPage("Index");
		}
	}
}