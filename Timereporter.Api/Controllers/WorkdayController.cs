using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timereporter.Core.Models;

namespace Timereporter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
		private readonly Collections.Workdays workdays;

		public WorkdayController(Collections.Workdays workdays)
		{
			this.workdays = workdays;
		}

		/// <summary>
		///  Example: curl -k -X GET https://localhost:44388/api/cursors/spelonki
		/// </summary>
		/// <param name="cursorType"></param>
		/// <returns></returns>
		[HttpGet("{year:int}/{month:int}")]
		public IActionResult Get(int year, int month)
		{
			var wd = workdays.Find(new Date(year, month, 1), new Date(year, month, DateTime.DaysInMonth(year, month)));
			return Ok(wd);
		}
	}
}
