﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyHaflinger.Web.Services;

namespace MyHaflinger.Web.Pages.Anmeldung.Verwaltung
{
	[Authorize]
	[RequireHttps]
	public class DownloadExcelModel : PageModel
	{
		public async Task<IActionResult> OnGetAsync([FromServices] AnmeldungsDbFactory dbFactory)
		{
			// https://github.com/closedxml/closedxml/wiki/Copying-IEnumerable-Collections
			var ctx = await dbFactory.CreateContextAsync();
			var registrations = await ctx.GetRegisteredParticipantsAsync();

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

			using (XLWorkbook wb = new XLWorkbook())
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

				ws.Cell(2, 1).InsertData(regTrimmed);

				// Title formatting
				var titlesStyle = wb.Style;
				titlesStyle.Font.Bold = true;
				titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
				titlesStyle.Fill.BackgroundColor = XLColor.DimGray;

				wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

				ws.Columns().AdjustToContents();

				var mStream = new MemoryStream();
				wb.SaveAs(mStream);
				mStream.Position = 0;

				return new FileStreamResult(mStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
				{
					FileDownloadName = "Teilnehmer.xlsx"
				};
			}
		}
	}
}