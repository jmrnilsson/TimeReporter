using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Timereporter.Core.Models;


namespace Timereporter.Api.Controllers
{


	[Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
		private readonly IApiService apiService;

		public ApiController(IApiService apiService)
		{
			this.apiService = apiService;
		}

		/// <summary>
		/// Example: curl -k -X GET http://localhost:53762/api/cursors/spelonki
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="escapedDateTimeZoneString">Escaped Nodatime DateTimeZoneId escaped with '/' to '_'</param>
		/// <returns></returns>
		[HttpGet("workday/{year:int}/{month:int}/{escapedDateTimeZoneString}")]
		public IActionResult Get(int year, int month, string escapedDateTimeZoneString)
		{
			DateTimeZone dateTimeZone = DateTimeZoneProviders.Tzdb[escapedDateTimeZoneString.Replace("_", "/")];
			var workdays = apiService.GetWorkdays(year, month, dateTimeZone);
			return Ok(workdays);
		}

		/// <summary>
		///  Example: curl -d "" -X POST http://localhost:53762/api/workday/calculate
		/// </summary>
		/// <returns></returns>
		[HttpPost("workday/calculate")]
		public IActionResult Calculate()
		{
			apiService.CalculateWorkdays();
			return Ok();
		}

		/// <summary>
		///  Example: curl -d "" -X PATCH http://localhost:53762/api/workday/calculate
		/// </summary>
		/// <returns></returns>
		[HttpPatch("workday/departure/{day}/{property:regex(^(arrival|break|departure)$)}/{hourOfDay:decimal}/{hashCode}")]
		public IActionResult Calculate(string day, string property, decimal hourOfDay, string hashCode)
		{
			DateText dateText = new DateText(day);

			if (!Enum.TryParse(property, out WorkdaySliceProperty property_))
			{
				return BadRequest();
			}

			return StatusCode((int)apiService.TrySaveWorkdaySlice(dateText, property_, hourOfDay, hashCode)); 
		}

		/// <summary>
		/// Example: curl -k -d "" -X POST https://localhost:44388/api/cursors/spelonki/15562
		/// </summary>
		/// <param name="cursorType"></param>
		/// <param name="position"></param>
		[HttpPatch("cursors/{cursorType}/{position:long}")]
		public void AddOrUpdate(string cursorType, long position)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///  Example: curl -k -X GET https://localhost:44388/api/cursors/spelonki
		/// </summary>
		/// <param name="cursorType"></param>
		/// <returns></returns>
		[HttpGet("cursors/{cursorType}")]
		public IActionResult Get(string cursorType)
		{
			var option = apiService.GetCursor(cursorType);
			return option.Match<IActionResult>(some: c => Ok(c), none: () => NotFound(cursorType));
		}

		[HttpGet("events/{from:long}/{to:long}")]
		public IEnumerable<Event> Find(long from, long to)
		{
			return apiService.GetEvents(from, to);
		}

		/// <summary>
		/// Moment in UNIX-timestamp. Example:
		/// curl -k -d "" -X POST https://localhost:44388/api/events/random/1556238242
		/// </summary>
		/// <param name="value"></param>
		[HttpPost("events")]
		public void Post(Events events)
		{
			apiService.SaveEvents(events.Events_);
		}

		/// <summary>
		/// curl -X GET https://localhost:44388/api/ping
		/// </summary>
		/// <returns></returns>
		[HttpGet("ping")]
		public IActionResult Ping()
		{
			return Ok();
		}
	}
}
