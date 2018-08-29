using System;
using System.IO;
using System.Threading.Tasks;
using MyHaflinger.Common.Services;
using MyHaflinger.Treffen.Db;
using RazorLight;

namespace MyHaflinger.Treffen.Services
{
	public class AnmeldungMailService
	{
		private readonly ISmtpMailService _smtpMailService;
		private readonly string _sender;

		public AnmeldungMailService(ISmtpMailService smtpMailService, string sender)
		{
			_smtpMailService = smtpMailService;
			_sender = sender;
		}

		private RazorLightEngine GetEngine()
		{
			return new RazorLightEngineBuilder()
				.UseMemoryCachingProvider()
				.Build();
		}

		private string GetTemplate(string templateKey)
		{
			string key = "MyHaflinger.Treffen.Templates." + templateKey + ".cshtml";

			using (var stream = GetType().Assembly.GetManifestResourceStream(key))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public async Task<bool> SendStep1Mail(string urlForContinuation, string to)
		{
			string template = GetTemplate("Anmeldung_Step1_Email");
			string msgToSend = await GetEngine().CompileRenderAsync("Anmeldung_Step1_Email", template, new { LinkToForm = urlForContinuation });
			return await _smtpMailService.SendMailAsync(to, "Emailadresse validiert, Haflingertreffen Salzkammergut", msgToSend, true, _sender);
		}

		public async Task<bool> SendRegCompleteMailToParticipant(Registration reg)
		{
			string template = GetTemplate("Anmeldung_FinalStep");
			string msgToSend = await GetEngine().CompileRenderAsync("Anmeldung_FinalStep", template, reg);
			return await _smtpMailService.SendMailAsync(reg.EmailAddress, "Anmeldebestätigung", msgToSend, true, _sender);
		}

		public async Task<bool> SendNewRegInfoToRegDesk(string to, Registration reg)
		{
			string template = GetTemplate("Anmeldung_New_2RegDesk");
			string msgToSend = await GetEngine().CompileRenderAsync("Anmeldung_New_2RegDesk", template, reg);
			return await _smtpMailService.SendMailAsync(to, "Neue Anmeldung", msgToSend, true, _sender);
		}
	}
}
