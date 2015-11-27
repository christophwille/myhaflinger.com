using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

public class SmtpMailService
{
    private readonly ConfigurationService _configurationService;

    public SmtpMailService(ConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public bool SendMail(string message, bool suppressExceptions = true)
    {
        try
        {
            string userName = _configurationService.SmtpUsername;
            string password = _configurationService.SmtpPassword;
            string host = _configurationService.SmtpHost;
            string port = _configurationService.SmtpPort;
            string sender = _configurationService.MailFromAddress;
            string to = _configurationService.ContactFormTo;

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
                From = new MailAddress(sender),
                Body = message,
                Subject = "myhaflinger.com Kontaktformular"
            };
            msg.To.Add(to);

            client.Send(msg);
            return true;
        }
        catch (Exception ex)
        {
            Trace.TraceError(ex.ToString());

            if (!suppressExceptions)
                throw;
        }

        return false;
    }
}