using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Timereporter.Api.Collections;
using Timereporter.Core;
using Timereporter.Core.Models;
using Timereporter.Core.Reducers;

namespace Timereporter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
		private readonly WorkdayRepository workdays;
		private readonly IEventPersistentLog eventPersistentLog;

		public WorkdayController(WorkdayRepository workdays, IEventPersistentLog eventPersistentLog)
		{
			this.workdays = workdays;
			this.eventPersistentLog = eventPersistentLog;
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
			var workdays_ = WorkdayReducer.ToWorkdayList(year, month, wd, tz);
			return Ok(workdays_);
		}

		/// <summary>
		///  Example: curl -d "" -X POST http://localhost:53762/api/workday/calculate
		/// </summary>
		/// <returns></returns>
		[HttpPost("calculate")]
		public IActionResult Calculate()
		{
			var tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			var fromSomeLocalDateToInstant = tz.AtStartOfDay(new LocalDate(2018, 10, 1)).ToInstant();
			var now = SystemClock.Instance.GetCurrentInstant();
			var events = eventPersistentLog.FindBy((fromSomeLocalDateToInstant, now));
			var workdaySlices = events.ToWorkdaySlices(tz);
			workdays.Save(workdaySlices.ToList());
			return Ok();
		}
	}
}
