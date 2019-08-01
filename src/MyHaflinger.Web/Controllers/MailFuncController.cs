using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
		private readonly ILogger<MailFuncController> _logger;

		public MailFuncController(AppOptions ao, ILogger<MailFuncController> logger)
		{
			_ao = ao;
			_logger = logger;
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
			try
			{
				var engine = new RazorLightEngineBuilder()
					.UseMemoryCachingProvider()
					.Build();

				string msgToSend = await engine.CompileRenderAsync("anfrageTemplateKey", AnfrageTemplate, mm);

				return await smtpMailService.SendMailAsync(_ao.ContactFormTo, "myhaflinger.com Kontaktformular", msgToSend, true, _ao.MailFromAddress);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return false;
		}
	}
}