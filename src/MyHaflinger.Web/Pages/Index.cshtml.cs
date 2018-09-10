using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Web.Common;

namespace MyHaflinger.Web.Pages
{
	public class IndexModel : MyHaflingerBasePageModel
	{
		// Caching is dealt with by base class
		// [ResponseCache(Duration = 3600,Location = ResponseCacheLocation.Any)]
		public void OnGet()
		{
		}
	}
}
