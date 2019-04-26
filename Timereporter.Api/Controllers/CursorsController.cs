using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timereporter.Api.Collections;
using Timereporter.Api.Entities;

namespace Timereporter.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CursorsController : ControllerBase
	{
		/// <summary>
		/// Example: curl -k -d "" -X POST https://localhost:44388/api/cursors/spelonki/15562
		/// </summary>
		/// <param name="cursorType"></param>
		/// <param name="position"></param>
		[HttpPost("{cursorType}/{position:long}")]
		[HttpPut("{cursorType}/{position:long}")]
		[HttpPatch("{cursorType}/{position:long}")]
		public void AddOrUpdate(string cursorType, long position)
		{
			var cursors = new Cursors();
			cursors.AddOrUpdate(new Cursor(cursorType, position));
		}

		/// <summary>
		///  Example: curl -k -X GET https://localhost:44388/api/cursors/spelonki
		/// </summary>
		/// <param name="cursorType"></param>
		/// <returns></returns>
		[HttpGet("{cursorType}")]
		public IActionResult Get(string cursorType)
		{
			var cursors = new Cursors();
			return cursors.GetBy(cursorType).Match(some: c => (IActionResult) Ok(c), none: () => NotFound(cursorType));
		}
	}
}
