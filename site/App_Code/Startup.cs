using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Startup))]

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        CookieSecureOption option = CookieSecureOption.Always;
        if (app.Properties.ContainsKey("host.AppMode") && (string)app.Properties["host.AppMode"] == "development")
        {
            option = CookieSecureOption.Never;
        }

        app.UseCookieAuthentication(new CookieAuthenticationOptions
        {
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            LoginPath = new PathString("/Anmeldung/Admin.Login/"),
            CookieHttpOnly = true,
            CookieSecure = option
        });

        // Use a cookie to temporarily store information about a user logging in with a third party login provider
        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
    }
}
