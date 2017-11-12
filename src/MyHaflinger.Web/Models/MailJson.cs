using System;
using System.Collections.Generic;
using System.Web;

namespace MyHaflinger.Web.Models
{
    public class MailJson
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}