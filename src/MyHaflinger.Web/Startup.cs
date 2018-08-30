using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHaflinger.Common.Services;
using MyHaflinger.Web.Models;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = new PathString("/Anmeldung/Verwaltung/Login");
				});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			AppOptions ao = new AppOptions();
			Configuration.Bind("AppOptions", ao);

			services.AddSingleton(ao);
			services.AddSingleton<ISmtpConfiguration>(ao);

			services.AddTransient<AnmeldungsDbFactory>();
			services.AddTransient<AnmeldungsAuthenticationFactory>();
			services.AddTransient<RegistrationFlowAuditTrailService>();
			
			services.AddTransient<ISmtpMailService, SmtpMailService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthentication();

			// app.UseMvc(); doesn't do it for MailFunc controller routing
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});
		}
	}
}

