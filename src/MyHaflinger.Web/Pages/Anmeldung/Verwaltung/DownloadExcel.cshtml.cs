using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	public class DownloadExcelModel : PageModel
	{
		public void OnGet([FromServices]AnmeldungsDbFactory dbFactory)
		{
			string fileName = "Teilnehmer.xlsx";

			Response.Clear();
			// Response.Headers.Clear();
			// Response.ClearContent();

			Response.Headers.Add("content-disposition", "attachment; filename=" + fileName);
			Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

			// https://github.com/closedxml/closedxml/wiki/Copying-IEnumerable-Collections
			var ctx = dbFactory.CreateContext();
			var registrations = ctx.GetRegisteredParticipants();

			var regTrimmed = registrations.Select(r => new
			{
				r.Id,
				r.RegisteredAt,
				r.Name,
				r.EmailAddress,
				r.Street,
				r.Zip,
				r.City,
				r.Country,
				r.Phone,
				r.NumberPlate,
				r.PParticipantsFriday,
				r.PParticipantsSatSun,
				r.TotalPrice,
				r.IntPaymentReceivedAmount,
				r.IntPaymentReceivedDate,
			}).OrderBy(r => r.Id);

			using (XLWorkbook wb = new XLWorkbook(XLEventTracking.Disabled))
			{
				var ws = wb.Worksheets.Add("Teilnehmer");

				ws.Cell(1, 1).Value = "Id";
				ws.Cell(1, 2).Value = "Reg Datum";
				ws.Cell(1, 3).Value = "Name";
				ws.Cell(1, 4).Value = "Email";
				ws.Cell(1, 5).Value = "Strasse";
				ws.Cell(1, 6).Value = "PLZ";
				ws.Cell(1, 7).Value = "Ort";
				ws.Cell(1, 8).Value = "Land";
				ws.Cell(1, 9).Value = "Tel";
				ws.Cell(1, 10).Value = "Kennzeichen";
				ws.Cell(1, 11).Value = "FR";
				ws.Cell(1, 12).Value = "SA/SO";
				ws.Cell(1, 13).Value = "Zu zahlen";
				ws.Cell(1, 14).Value = "Bezahlt";
				ws.Cell(1, 15).Value = "Z-Datum";

				ws.Range(1, 1, 1, 15).AddToNamed("Titles");

				ws.Cell(2, 1).Value = regTrimmed;

				// Title formatting
				var titlesStyle = wb.Style;
				titlesStyle.Font.Bold = true;
				titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				titlesStyle.Fill.BackgroundColor = XLColor.DimGray;

				wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

				ws.Columns().AdjustToContents();

				using (MemoryStream mStream = new MemoryStream())
				{
					wb.SaveAs(mStream);
					
					mStream.WriteTo(HttpContext.Response.Body);
					HttpContext.Response.Body.Flush();
					// HttpContext.Response.Body.EndWrite();
				}
			}
		}
	}
}