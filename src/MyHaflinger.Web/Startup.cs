using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
			// https://dotnetcoretutorials.com/2017/06/22/request-culture-asp-net-core/
			// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#implement-a-strategy-to-select-the-languageculture-for-each-request
			services.Configure<RequestLocalizationOptions>(options =>
			{
				options.DefaultRequestCulture = new RequestCulture("de");
				options.SupportedCultures = new List<CultureInfo> { new CultureInfo("de") };
				options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("de") };
				options.RequestCultureProviders = new List<IRequestCultureProvider>();
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = new PathString("/Anmeldung/Verwaltung/Login");
				});

			services.AddResponseCaching();

			services.AddControllers();
			services.AddRazorPages()
				.AddRazorRuntimeCompilation();

			services.Configure<MvcOptions>(options =>
			{
				options.Filters.Add(new RequireHttpsAttribute { Permanent = true });
				// options.ReturnHttpNotAcceptable = false;

				// https://docs.microsoft.com/en-us/aspnet/core/performance/caching/response?view=aspnetcore-3.1#responsecache-attribute
				options.CacheProfiles.Add("Default",
					new CacheProfile()
					{
						Duration = 3600,
						Location = ResponseCacheLocation.Client
					});
			});

			AppOptions ao = new AppOptions();
			Configuration.Bind("AppOptions", ao);

			services.AddSingleton(ao);
			services.AddSingleton<ISmtpConfiguration>(ao);

			services.AddTransient<AnmeldungsDbFactory>();
			services.AddTransient<AnmeldungsAuthenticationFactory>();
			services.AddTransient<RegistrationFlowAuditTrailService>();

			services.AddTransient<ISmtpMailService, SmtpMailService>();
			services.AddTransient<ITemplateRenderingService, HandlebarsRenderingService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseRequestLocalization();

			app.UseHttpsRedirection();

			// If the app calls UseStaticFiles, place UseStaticFiles before UseRouting.
			app.UseStaticFiles();

			app.UseRouting();

			// If the app uses authentication / authorization features such as AuthorizePage or[Authorize], place the call to UseAuthentication and UseAuthorization after UseRouting.
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseResponseCaching();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
			});
		}
	}
}

