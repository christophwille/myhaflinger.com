using MyHaflinger.Common.Services;
using MyHaflinger.Web.Models;

namespace MyHaflinger.Web.Services
{
	public class ContactUsService
	{
		private readonly ISmtpMailService _smtpMailService;
		private readonly ITemplateRenderingService _templateRenderer;
		private readonly AppOptions _ao;
		private readonly ILogger<ContactUsService> _logger;

		public ContactUsService(ISmtpMailService smtpMailService, ITemplateRenderingService templateRenderer,
			AppOptions ao, ILogger<ContactUsService> logger)
		{
			_smtpMailService = smtpMailService;
			_templateRenderer = templateRenderer;
			_ao = ao;
			_logger = logger;
		}

		public async Task<bool> SendAnfrageFormularMail(MailJson mm)
		{
			if (string.IsNullOrEmpty(mm.Name) || string.IsNullOrEmpty(mm.Email) ||
				string.IsNullOrEmpty(mm.Subject) || string.IsNullOrEmpty(mm.Message))
			{
				return false;
			}

			try
			{
				string msgToSend = _templateRenderer.Render(AnfrageTemplate, mm);

				return await _smtpMailService.SendMailAsync(_ao.ContactFormTo, "myhaflinger.com Kontaktformular", msgToSend, true, _ao.MailFromAddress);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}

			return true;
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
	}
}