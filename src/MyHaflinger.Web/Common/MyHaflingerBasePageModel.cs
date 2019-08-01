using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace MyHaflinger.Web.Common
{
	public class MyHaflingerBasePageModel : PageModel
	{
		public bool IsCachingEnabled { get; set; } = true;

		/// <summary>
		/// This only works when the Pages are deployed, not precompiled. MvcRazorCompileOnPublish must be set to false in the .csproj file
		/// </summary>
		/// <param name="context"></param>
		public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
		{
			if (IsCachingEnabled && !context.HttpContext.IsLocalRequest())
			{
				context.HttpContext.Response.GetTypedHeaders().CacheControl =
					new CacheControlHeaderValue()
					{
						Public = true,
						MaxAge = TimeSpan.FromHours(6)
					};

				// https://stackoverflow.com/questions/52151452/request-currentexecutionfilepath-in-asp-net-core/52152027#52152027
				string cshtmlFilePath = context.ActionDescriptor.RelativePath; // returns e.g. /Pages/Index.cshtml

				var env = context.HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;
				string fullPath = Path.Combine(env.ContentRootPath, cshtmlFilePath.Substring(1)); // must trim leading '/' otherwise absolute to root drive of app
				var fileInfo = new FileInfo(fullPath);

				if (fileInfo.Exists)
				{
					// https://stackoverflow.com/questions/50509221/addfiledependency-alternative-in-asp-net-core
					context.HttpContext.Response.GetTypedHeaders().LastModified = fileInfo.LastWriteTimeUtc;
				}
			}
		}
	}
}
