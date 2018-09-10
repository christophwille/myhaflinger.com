using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Common;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[RequireHttps]
	public class HashPasswordModel : PageModel
    {
	    [BindProperty]
	    [Required]
	    public string Password { get; set; }
		public string Hash { get; set; }

		public void OnGet()
        {

        }

	    public IActionResult OnPost()
	    {
		    if (!ModelState.IsValid) return Page();

		    Hash = PBKDF2.HashPassword(Password);

		    return Page();
	    }
    }
}