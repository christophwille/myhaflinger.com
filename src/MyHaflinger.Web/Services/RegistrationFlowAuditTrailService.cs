using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using MiscUtil.IO;

namespace MyHaflinger.Web.Services
{
	public class RegistrationFlowAuditTrailService
	{
		private readonly string LoggerName = "RegistrierungsTrace";

		public List<string> GetTailOfLog(int numOfRecords = 100)
		{
			var location = System.Reflection.Assembly.GetEntryAssembly().Location;
			var webRoot = System.IO.Path.GetDirectoryName(location);

			var anmeldungsLogPath = System.IO.Path.Combine(webRoot, "Data", "anmeldunglog.txt");

			// Taken from: http://jonskeet.uk/csharp/miscutil/ via https://stackoverflow.com/questions/10409977/how-to-efficiently-read-only-last-line-of-the-text-file
			var lines = new ReverseLineReader(anmeldungsLogPath);
			return lines.Take(numOfRecords).ToList(); // .Reverse() if you want chronological order;
		}

		public void Trace(string traceMsg)
		{
			var logger = NLog.LogManager.GetLogger(LoggerName);
			logger.Trace(traceMsg);
		}
	}
}
