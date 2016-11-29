using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace MyHaflinger.Anmeldung
{
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
}