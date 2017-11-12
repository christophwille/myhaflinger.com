using NLog;
using System;
using System.Net;
using System.Net.Mail;

namespace MyHaflinger.Web.Services
{
    public class SmtpMailService
    {
        private readonly ConfigurationService _configurationService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SmtpMailService(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public bool SendMail(string mailSubject, string message, bool isBodyHtml, bool suppressExceptions = true)
        {
            string to = _configurationService.ContactFormTo;
            return SendMail(mailSubject, message, isBodyHtml, to, suppressExceptions);
        }

        public bool SendMail(string mailSubject, string message, bool isBodyHtml, string to,
            bool suppressExceptions = true)
        {
            try {
                string userName = _configurationService.SmtpUsername;
                string password = _configurationService.SmtpPassword;
                string host = _configurationService.SmtpHost;
                string port = _configurationService.SmtpPort;
                string sender = _configurationService.MailFromAddress;

                int portNumeric = 25;
                Int32.TryParse(port, out portNumeric);

                var client = new SmtpClient() {
                    Credentials = new NetworkCredential(userName, password),
                    Host = host,
                    Port = portNumeric
                };

                var msg = new System.Net.Mail.MailMessage {
                    From = new MailAddress(sender),
                    Body = message,
                    IsBodyHtml = isBodyHtml,
                    Subject = mailSubject
                };
                msg.To.Add(to);

                client.Send(msg);
                return true;
            } catch (Exception ex) {
                logger.Error(ex.ToString);

                if (!suppressExceptions)
                    throw;
            }

            return false;
        }
    }
}