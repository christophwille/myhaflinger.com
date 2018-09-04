using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace MyHaflinger.Web.Common
{
	public static class HttpContextExtensions
	{
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
