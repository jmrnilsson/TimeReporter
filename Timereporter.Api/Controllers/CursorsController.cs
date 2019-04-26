using System;
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
		[HttpPost("{cursorType}/{position:long}")]
		[HttpPut("{cursorType}/{position:long}")]
		[HttpPatch("{cursorType}/{position:long}")]
		public void AddOrUpdate(string cursorType, long position)
		{
			var cursors = new Cursors();
			cursors.AddOrUpdate(new Cursor(cursorType, position));
		}

		[HttpGet("{cursorType}")]
		public IActionResult Get(string cursorType)
		{
			var cursors = new Cursors();
			var cursor = cursors.GetBy(cursorType);

			if (cursor == null)
			{
				return NotFound(cursorType);
			}

			return Ok(cursor);
		}
	}
}
