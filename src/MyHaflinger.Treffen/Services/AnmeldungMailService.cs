using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using MyHaflinger.Common.Services;
using MyHaflinger.Treffen.Db;

namespace MyHaflinger.Treffen.Services
{
	public class AnmeldungMailService
	{
		private static readonly ConcurrentDictionary<string,string> Templates = new ConcurrentDictionary<string, string>();

		private readonly ISmtpMailService _smtpMailService;
		private readonly ITemplateRenderingService _templateRenderer;
		private readonly string _sender;

		public AnmeldungMailService(ISmtpMailService smtpMailService, ITemplateRenderingService templateRenderer, string sender)
		{
			_smtpMailService = smtpMailService;
			_templateRenderer = templateRenderer;
			_sender = sender;
		}

		private string GetTemplate(string templateKey)
		{
			string key = "MyHaflinger.Treffen.Templates." + templateKey + ".cshtml";

			if (!Templates.TryGetValue(key, out string templateValue))
			{
				using (var stream = GetType().Assembly.GetManifestResourceStream(key))
				{
					using (var reader = new StreamReader(stream))
					{
						templateValue = reader.ReadToEnd();
					}
				}

				Templates.TryAdd(key, templateValue);
			}

			return templateValue;
		}

		public async Task<bool> SendStep1Mail(string urlForContinuation, string to)
		{
			string template = GetTemplate("Anmeldung_Step1_Email");
			string msgToSend = _templateRenderer.Render(template, new { LinkToForm = urlForContinuation });
			return await _smtpMailService.SendMailAsync(to, "Emailadresse validiert, Haflingertreffen Salzkammergut", msgToSend, true, _sender);
		}

		public async Task<bool> SendRegCompleteMailToParticipant(Registration reg)
		{
			string template = GetTemplate("Anmeldung_FinalStep");
			string msgToSend = _templateRenderer.Render(template, reg);
			return await _smtpMailService.SendMailAsync(reg.EmailAddress, "Anmeldebestätigung", msgToSend, true, _sender);
		}

		public async Task<bool> SendNewRegInfoToRegDesk(string to, Registration reg)
		{
			string template = GetTemplate("Anmeldung_New_2RegDesk");
			string msgToSend = _templateRenderer.Render(template, reg);
			return await _smtpMailService.SendMailAsync(to, "Neue Anmeldung", msgToSend, true, _sender);
		}
	}
}
