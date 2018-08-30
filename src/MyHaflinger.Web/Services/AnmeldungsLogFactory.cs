using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MiscUtil.IO;

namespace MyHaflinger.Web.Services
{
	public class AnmeldungsLogFactory
	{
		private readonly string _anmeldungsLogPath;
		public AnmeldungsLogFactory(IHostingEnvironment env)
		{
			// var webRoot = env.ContentRootPath;
			var location = System.Reflection.Assembly.GetEntryAssembly().Location;
			var webRoot = System.IO.Path.GetDirectoryName(location);

			_anmeldungsLogPath = System.IO.Path.Combine(webRoot, "Data", "anmeldunglog.txt");
		}

		public List<string> GetTailOfLog(int numOfRecords = 100)
		{
			// Taken from: http://jonskeet.uk/csharp/miscutil/ via https://stackoverflow.com/questions/10409977/how-to-efficiently-read-only-last-line-of-the-text-file
			var lines = new ReverseLineReader(_anmeldungsLogPath);
			return lines.Take(numOfRecords).ToList(); // .Reverse() if you want chronological order;
		}
	}
}
