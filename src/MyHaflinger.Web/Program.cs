using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using MyHaflinger.Common.Services;
using MyHaflinger.Web.Models;
using MyHaflinger.Web.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
#if !DEBUG
	builder.Logging.ClearProviders();
#endif
builder.Logging.AddNLogWeb();

// https://dotnetcoretutorials.com/2017/06/22/request-culture-asp-net-core/
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#implement-a-strategy-to-select-the-languageculture-for-each-request
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	options.DefaultRequestCulture = new RequestCulture("de");
	options.SupportedCultures = new List<CultureInfo> { new CultureInfo("de") };
	options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("de") };
	options.RequestCultureProviders = new List<IRequestCultureProvider>();
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = new PathString("/Anmeldung/Verwaltung/Login");
	});

builder.Services.AddResponseCaching();

builder.Services.AddRazorPages()
	.AddRazorRuntimeCompilation();

builder.Services.Configure<MvcOptions>(options =>
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

var ao = builder.Configuration.GetSection("AppOptions").Get<AppOptions>();

builder.Services.AddSingleton(ao);
builder.Services.AddSingleton<ISmtpConfiguration>(ao);

builder.Services.AddTransient<AnmeldungsDbFactory>();
builder.Services.AddTransient<AnmeldungsAuthenticationFactory>();
builder.Services.AddTransient<RegistrationFlowAuditTrailService>();

builder.Services.AddTransient<ISmtpMailService, SmtpMailService>();
builder.Services.AddTransient<ITemplateRenderingService, HandlebarsRenderingService>();
builder.Services.AddTransient<ContactUsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// app.UseHsts();
}

app.UseRequestLocalization();

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

app.MapRazorPages();

app.MapPost("/mailfunc", async ([FromBody] MailJson mm, [FromServices] ContactUsService contactService) =>
{
	bool ok = await contactService.SendAnfrageFormularMail(mm);

	return Results.Ok();
});

app.Run();
