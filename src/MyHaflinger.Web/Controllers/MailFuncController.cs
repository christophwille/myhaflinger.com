using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyHaflinger.Common.Services;
using MyHaflinger.Web.Models;
using MyHaflinger.Web.Services;
using RazorLight;
using RazorLight.Razor;

namespace MyHaflinger.Web.Controllers
{
	public class MailFuncController : Controller
	{
		private readonly AppOptions _ao;

		public MailFuncController(AppOptions ao)
		{
			_ao = ao;
		}

		[HttpPost]
		public async Task<IActionResult> Index([FromBody]MailJson mm, [FromServices] ISmtpMailService smtpMailService)
		{
			if (String.IsNullOrEmpty(mm.Name) || String.IsNullOrEmpty(mm.Email) ||
				String.IsNullOrEmpty(mm.Subject) || String.IsNullOrEmpty(mm.Message))
			{
				// TODO: return status code?
			}
			else
			{
				await SendAnfrageFormularMail(mm, smtpMailService);
			}

			return Ok();
		}

		private static readonly string AnfrageTemplate = @"<!DOCTYPE html>
					<html>
					<body>
						<h1>Anfrage myhaflinger.com</h1>
						<p>Name: @Model.Name</p>
						<p>Email: @Model.Email</p>
						<p>Betreff: @Model.Subject</p>
						<p>Nachricht:</p>
						<p>@Model.Message</p>
					</body>
					</html>";

		public async Task<bool> SendAnfrageFormularMail(MailJson mm, ISmtpMailService smtpMailService)
		{
			var engine = new EngineFactory().Create(new FileSystemRazorProject("/"), null);
			string msgToSend = await engine.CompileRenderAsync("anfrageTemplateKey", AnfrageTemplate, mm);

			return await smtpMailService.SendMailAsync(_ao.ContactFormTo, "myhaflinger.com Kontaktformular", msgToSend, true, _ao.MailFromAddress);
		}
	}
}