using System.Threading.Tasks;

namespace MyHaflinger.Common.Services
{
	public interface ISmtpMailService
	{
		Task<bool> SendMailAsync(string to, string mailSubject, string message, bool isBodyHtml, string from, bool suppressExceptions = true);
	}
}