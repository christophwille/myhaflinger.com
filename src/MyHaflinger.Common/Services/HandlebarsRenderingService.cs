using System;
using System.Collections.Generic;
using System.Text;
using HandlebarsDotNet;

namespace MyHaflinger.Common.Services
{
	public class HandlebarsRenderingService : ITemplateRenderingService
	{
		public string Render(string source, object data)
		{
			Func<object, string> template = Handlebars.Compile(source);
			string result = template(data);
			return result;
		}
	}
}
