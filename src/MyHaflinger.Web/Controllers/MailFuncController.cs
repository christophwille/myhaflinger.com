using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyHaflinger.Common.Services;
using MyHaflinger.Web.Models;

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
		public async Task<IActionResult> Index([FromBody]MailJson mm, [FromServices] ISmtpMailService smtpMailService, [FromServices] ITemplateRenderingService templateRenderer)
		{
			if (String.IsNullOrEmpty(mm.Name) || String.IsNullOrEmpty(mm.Email) ||
				String.IsNullOrEmpty(mm.Subject) || String.IsNullOrEmpty(mm.Message))
			{
				// TODO: return status code?
			}
			else
			{
				await SendAnfrageFormularMail(mm, smtpMailService, templateRenderer);
			}

			return Ok();
		}

		private static readonly string AnfrageTemplate = @"<!DOCTYPE html>
		<html>
		<body>
			<h1>Anfrage myhaflinger.com</h1>
			<p>Name: {{Name}}</p>
			<p>Email: {{Email}}</p>
			<p>Betreff: {{Subject}}</p>
			<p>Nachricht:</p>
			<p>{{Message}}</p>
		</body>
		</html>";

		public async Task<bool> SendAnfrageFormularMail(MailJson mm, ISmtpMailService smtpMailService, ITemplateRenderingService templateRenderer)
		{
			try
			{
				string msgToSend = templateRenderer.Render(AnfrageTemplate, mm);

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