using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHaflinger.Common.Services;

namespace MyHaflinger.Web.Models
{
	public class AppOptions : ISmtpConfiguration
	{
		public string ContactFormTo { get; set; }
		public string MailFromAddress { get; set; }
		public string SmtpUsername { get; set; }
		public string SmtpPassword { get; set; }
		public string SmtpHost { get; set; }
		public string SmtpPort { get; set; }
		public bool RegistrationOpen { get; set; }
	} 
}
