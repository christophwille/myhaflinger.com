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
        public static readonly string AnfrageTemplate = @"<!DOCTYPE html>
                    <html>
                    <body>
                        <h1>Anfrage myhaflinger.com</h1>
                        <p>Name: @Model.Name</p>
                        <p>Email: @Model.Email</p>
                        <p>Betreff: @Model.Subject</p>
                        <p>Nachricht:</p>
                        <p>@Model.Message</p>
                    </body>
                    </html>";

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