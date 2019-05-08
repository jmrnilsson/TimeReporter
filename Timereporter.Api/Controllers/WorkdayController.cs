using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Timereporter.Api.Collections;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
		private readonly WorkdayRepository workdays;
		private readonly WorkdaySummarizer workdaySummarizer;

		public WorkdayController(WorkdayRepository workdays, WorkdaySummarizer workdaySummarizer)
		{
			this.workdays = workdays;
			this.workdaySummarizer = workdaySummarizer;
		}

		/// <summary>
		///  Example: curl -k -X GET https://localhost:44388/api/cursors/spelonki
		/// </summary>
		/// <param name="cursorType"></param>
		/// <returns></returns>
		[HttpGet("{year:int}/{month:int}/{dtz}")]
		public IActionResult Get(int year, int month, string dtz)
		{
			// var tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			DateTimeZone tz = DateTimeZoneProviders.Tzdb[dtz.Replace("_", "/")];
			var fromLocalDate = tz.AtStartOfDay(new LocalDate(year, month, 1));
			var exclusiveToLocalDate = tz.AtStartOfDay(new LocalDate(year, month, DateTime.DaysInMonth(year, month)).PlusDays(1));
			List<Event> wd = workdays.Find(fromLocalDate.ToInstant(), exclusiveToLocalDate.ToInstant());
			var workdays_ = workdaySummarizer.Summarize(year, month, wd, tz);
			return Ok(workdays_);
		}

		[HttpGet("arrival/{minutes:int}")]
		public IActionResult Get(int minutes)
		{
			throw new NotImplementedException();
			//var tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			//var fromLocalDate = tz.AtStartOfDay(new LocalDate(year, month, 1));
			//var exclusiveToLocalDate = tz.AtStartOfDay(new LocalDate(year, month, DateTime.DaysInMonth(year, month)).PlusDays(1));
			//var wd = workdays.Find(fromLocalDate.ToInstant(), exclusiveToLocalDate.ToInstant());
			//return Ok(Extensions.ToWorkdays(wd, tz));
		}
	}
}
