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
			var template = Handlebars.Compile(source);
			return template(data);
		}
	}
}
