namespace MyHaflinger.Common.Services
{
	public interface ISmtpConfiguration
	{
		string SmtpUsername { get; set; }
		string SmtpPassword { get; set; }
		string SmtpHost { get; set; }
		string SmtpPort { get; set; }
	}
}