using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHaflinger.Web.Models
{
    public class AppOptions
    {
        public string ContactFormTo { get; set; }
        public string MailFromAddress { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpPort { get; set; }
    } 
}
