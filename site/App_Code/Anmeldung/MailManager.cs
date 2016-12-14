using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;
using MyHaflinger.Anmeldung.Data;

namespace MyHaflinger
{
    public static partial class MailManager
    {
        public static bool SendStep1Mail(string urlForContinuation, string to)
        {
            string templateHtml = ReadHtmlTemplate("Anmeldung_Step1_Email.html");
            var msgToSend = Engine.Razor.RunCompile(templateHtml, "anmeldungStep1TemplateKey", null, new { LinkToForm = urlForContinuation });

            var emailSvc = new SmtpMailService(new ConfigurationService());
            return emailSvc.SendMail("Emailadresse validiert, Haflingertreffen Salzkammergut", msgToSend, true, to);
        }

        public static bool SendRegCompleteMailToParticipant(Registration reg)
        {
            string templateHtml = ReadHtmlTemplate("Anmeldung_FinalStep.html");
            var msgToSend = Engine.Razor.RunCompile(templateHtml, "anmeldungFinalStepTemplateKey", null, reg);

            var emailSvc = new SmtpMailService(new ConfigurationService());
            return emailSvc.SendMail("Anmeldebestätigung", msgToSend, true, reg.EmailAddress);
        }

        public static bool SendNewRegInfoToRegDesk(string to, Registration reg)
        {
            string templateHtml = ReadHtmlTemplate("Anmeldung_New_2RegDesk.html");
            var msgToSend = Engine.Razor.RunCompile(templateHtml, "anmeldungRegDeskInfoTemplateKey", null, reg);

            var emailSvc = new SmtpMailService(new ConfigurationService());
            return emailSvc.SendMail("Neue Anmeldung", msgToSend, true, to);
        }
    }
}
