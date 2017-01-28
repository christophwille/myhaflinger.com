using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Principal;
using System.Web;

namespace MyHaflinger.Anmeldung
{
    public static class AnmeldungRoles
    {
        private static string Kassier = "kassier";
        private static string Veranstaltungswart = "wart";

        public static bool IsKassier(this IPrincipal p)
        {
            return p.IsInRole(Kassier);
        }

        public static bool IsVeranstaltungswart(this IPrincipal p)
        {
            return p.IsInRole(Veranstaltungswart);
        }
    }

    public static class AuthenticationFactory
    {
        public static ClaimsIdentity CreateIdentity(string username, string role)
        {
            var claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, username));
            claimsIdentity.AddClaim(
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                        "ASP.NET Identity",
                        "http://www.w3.org/2001/XMLSchema#string"));

            var adminClaim = new Claim(ClaimTypes.Role, role);
            claimsIdentity.AddClaim(adminClaim);

            return claimsIdentity;
        }
    }

    public static class AdminPanel
    {
        public static void RequireSsl()
        {
            var owinContext = System.Web.HttpContext.Current.GetOwinContext();

            if (owinContext.Request.Uri.IsLoopback) return;

            if (owinContext.Request.Uri.Scheme != Uri.UriSchemeHttps)
            {
                var builder = new UriBuilder(owinContext.Request.Uri) {Scheme = Uri.UriSchemeHttps, Port = -1};
                owinContext.Response.Redirect(builder.ToString());
            }
        }

        public static void RequireAuthN(IPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                System.Web.HttpContext.Current.GetOwinContext().Authentication.Challenge();
            }
        }

        public static void RequireAuthNKassierAuthZ(IPrincipal user)
        {
            if (!user.Identity.IsAuthenticated || !user.IsKassier())
            {
                System.Web.HttpContext.Current.GetOwinContext().Authentication.Challenge();
            }
        }
    }
}