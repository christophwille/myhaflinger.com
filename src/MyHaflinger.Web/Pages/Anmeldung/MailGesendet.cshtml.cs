using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyHaflinger.Web.Pages.Anmeldung
{
	public class MailGesendetModel : PageModel
	{
		public string Email { get; set; }
		public string FormularUrl { get; set; }

		public void OnGet(string guid, string email)
		{
			Email = email;
			FormularUrl = StartModel.GenerateFormularUrl(HttpContext, guid);
		}
	}
}