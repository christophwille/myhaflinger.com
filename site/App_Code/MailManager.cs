using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RazorEngine;
using RazorEngine.Templating;

namespace MyHaflinger
{
    public static partial class MailManager
    {
        public static readonly string AnfrageTemplate = "Name: @Model.Name\r\nEmail: @Model.Email\r\nBetreff: @Model.Subject\r\nNachricht: \r\n@Model.Message\r\n";

        public static void SendAnfrageFormularMail(MailJson mm)
        {
            // https://github.com/Antaris/RazorEngine
            var msgToSend = Engine.Razor.RunCompile(AnfrageTemplate, "anfrageTemplateKey", null, mm);

            var emailSvc = new SmtpMailService(new ConfigurationService());
            emailSvc.SendMail("myhaflinger.com Kontaktformular", msgToSend, isBodyHtml:true);
        }

        private static string ReadHtmlTemplate(string templateFileName)
        {
            string filename = HttpContext.Current.Server.MapPath("~/App_Data/" + templateFileName);
            return System.IO.File.ReadAllText(filename);
        }
    }
}