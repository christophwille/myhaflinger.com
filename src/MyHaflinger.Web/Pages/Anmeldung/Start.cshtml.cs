﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Common.Services;
using MyHaflinger.Treffen.Services;
using MyHaflinger.Web.Models;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung
{
	[RequireHttps]
	public class StartModel : PageModel
	{
		private readonly AnmeldungsDbFactory _dbFactory;
		private readonly AppOptions _ao;

		public StartModel(AnmeldungsDbFactory dbFactory, AppOptions ao)
		{
			_dbFactory = dbFactory;
			_ao = ao;
		}

		public IActionResult OnGet()
		{
			if (!_ao.RegistrationOpen) return RedirectToPage("/Index");

			return Page();
		}

		public static string GenerateFormularUrl(HttpContext ctx, string id)
		{
			string scheme = ctx.Request.Scheme;
			string host = ctx.Request.Host.Value;
			return $"{scheme}://{host}/Anmeldung/Formular?token={id}";
		}

		[BindProperty]
		[RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Ungültiges Format für die Emailadresse")]
		[Required(ErrorMessage = "Emailadresse muß eingegeben werden")]
		public string EmailAddress { get; set; }

		public async Task<IActionResult> OnPostAsync([FromServices]ISmtpMailService smtpMailService, [FromServices]ITemplateRenderingService templateRenderer, [FromServices]RegistrationFlowAuditTrailService auditLog)
		{
			if (!ModelState.IsValid) return Page();

			string guid = Guid.NewGuid().ToString();

			var ctx = await _dbFactory.CreateContextAsync();
			await ctx.RegisterEmailChallengeAsync(EmailAddress, guid, DateTime.UtcNow);

			auditLog.Trace($"REG:S:CH: Email {EmailAddress} registered for challenge");

			string formularUrl = GenerateFormularUrl(HttpContext, guid);

			var mm = new AnmeldungMailService(smtpMailService, templateRenderer, _ao.MailFromAddress);
			bool mailSentOk = await mm.SendStep1Mail(formularUrl, EmailAddress);

			if (!mailSentOk)
			{
				ModelState.AddModelError("mailfail", $"Mail an {EmailAddress} konnte nicht gesendet werden. Vertippt?");
				return Page();
			}

			return RedirectToPage("MailGesendet", new { guid, email = EmailAddress });
		}
	}
}