using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MiscUtil.IO;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[Authorize]
	public class TailOfLogModel : PageModel
	{
		public List<string> TailingLogLines { get; private set; }

		public void OnGet([FromServices]RegistrationFlowAuditTrailService logFactory)
		{
			TailingLogLines = logFactory.GetTailOfLog();
		}
	}
}