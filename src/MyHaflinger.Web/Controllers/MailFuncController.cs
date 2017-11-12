using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyHaflinger.Web.Models;

namespace MyHaflinger.Web.Controllers
{
    public class MailFuncController : Controller
    {
        [HttpPost]
        public IActionResult Index([FromBody]MailJson mm)
        {
            //if (String.IsNullOrEmpty(mm.Name) || String.IsNullOrEmpty(mm.Email) ||
            //    String.IsNullOrEmpty(mm.Subject) || String.IsNullOrEmpty(mm.Message)) {
            //    // TODO: return status code?
            //} else {
            //    MyHaflinger.MailManager.SendAnfrageFormularMail(mm);
            //}

            return Ok();
        }
    }
}