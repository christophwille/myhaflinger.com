using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyHaflinger.Common.Services
{
	public interface ISmtpConfiguration
	{
		string SmtpUsername { get; set; }
		string SmtpPassword { get; set; }
		string SmtpHost { get; set; }
		string SmtpPort { get; set; }
	}

	public interface ISmtpMailService
	{
		Task<bool> SendMailAsync(string to, string mailSubject, string message, bool isBodyHtml, string from, bool suppressExceptions = true);
	}

	public class SmtpMailService : ISmtpMailService
	{
		private readonly ISmtpConfiguration _smtpConfig;
		private readonly ILogger _log;

		public SmtpMailService(ISmtpConfiguration smtpConfig, ILogger<SmtpMailService> log)
		{
			_smtpConfig = smtpConfig;
			_log = log;
		}

		public async Task<bool> SendMailAsync(string to, string mailSubject, string message, bool isBodyHtml, string from, bool suppressExceptions = true)
		{
			try
			{
				string userName = _smtpConfig.SmtpUsername;
				string password = _smtpConfig.SmtpPassword;
				string host = _smtpConfig.SmtpHost;
				string port = _smtpConfig.SmtpPort;

				int portNumeric = 25;
				Int32.TryParse(port, out portNumeric);

				var client = new SmtpClient()
				{
					Credentials = new NetworkCredential(userName, password),
					Host = host,
					Port = portNumeric
				};

				var msg = new System.Net.Mail.MailMessage
				{
					From = new MailAddress(from),
					Body = message,
					IsBodyHtml = isBodyHtml,
					Subject = mailSubject
				};
				msg.To.Add(to);

				await client.SendMailAsync(msg);
				return true;
			}
			catch (Exception ex)
			{
				_log.LogError(ex.ToString());

				if (!suppressExceptions)
					throw;
			}

			return false;
		}
	}
}