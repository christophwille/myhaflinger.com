using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IO;

namespace MyHaflinger.Web.Common
{
	public static class HttpContextExtensions
	{
		public static void CacheWhenNotLocal(this HttpContext context)
		{
			if (!context.IsLocalRequest())
			{
				context.Response.GetTypedHeaders().CacheControl =
					new CacheControlHeaderValue()
					{
						Public = true,
						MaxAge = TimeSpan.FromMinutes(60)
					};
				
				//var env = context.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
				//var fileInfo = new FileInfo(Path.Combine(env.ContentRootPath, context.Request.Path));
				//context.Response.GetTypedHeaders().LastModified = fileInfo.LastWriteTimeUtc;
			}
		}

		// https://stackoverflow.com/questions/35240586/in-asp-net-core-how-do-you-check-if-request-is-local

		public static bool IsLocalRequest(this HttpContext context)
		{
			if (context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress))
			{
				return true;
			}
			if (IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
			{
				return true;
			}
			return false;
		}
	}
}
