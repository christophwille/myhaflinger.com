using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung
{
    public class StartModel : PageModel
    {
        private AnmeldungsDbFactory DbFactory { get; }

        public StartModel(AnmeldungsDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        [BindProperty]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")]
        [Required]
        public string EmailAddress { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            string guid = Guid.NewGuid().ToString();

            var ctx = DbFactory.CreateContext();
            ctx.RegisterEmailChallenge(EmailAddress, guid, DateTime.UtcNow);

            var logger = NLog.LogManager.GetLogger("RegistrierungsTrace");
            logger.Trace("REG:S:CH: Email {0} registered for challenge", EmailAddress);

            string scheme = HttpContext.Request.Scheme;
            string host = HttpContext.Request.Host.Value;
            string formularUrl = $"{scheme}://{host}/Anmeldung/Formular?token={guid}";

            bool mailSentOk = false; // TODO: MyHaflinger.MailManager.SendStep1Mail(formularUrl, email);
            if (!mailSentOk)
            {
                ModelState.AddModelError("mailfail", $"Mail an {EmailAddress} konnte nicht gesendet werden. Vertippt?");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}