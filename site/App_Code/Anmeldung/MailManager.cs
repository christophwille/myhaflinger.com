﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;

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
    }
}
