using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timereporter.Api.Collections;
using Timereporter.Core.Models;
using Optional;
using System;

namespace Timereporter.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CursorsController : ControllerBase
	{
		private readonly ICursorRepository cursors;

		public CursorsController(ICursorRepository cursors)
		{
			this.cursors = cursors;
		}

		/// <summary>
		/// Example: curl -k -d "" -X POST https://localhost:44388/api/cursors/spelonki/15562
		/// </summary>
		/// <param name="cursorType"></param>
		/// <param name="position"></param>
		[HttpPatch("{cursorType}/{position:long}")]
		public void AddOrUpdate(string cursorType, long position)
		{
			throw new NotImplementedException();
			// cursors.Save(new Cursor(cursorType, position));
		}

		/// <summary>
		///  Example: curl -k -X GET https://localhost:44388/api/cursors/spelonki
		/// </summary>
		/// <param name="cursorType"></param>
		/// <returns></returns>
		[HttpGet("{cursorType}")]
		public IActionResult Get(string cursorType)
		{
			var option = cursors.Find(cursorType);
			return option.Match<IActionResult>(some: c => Ok(c), none: () => NotFound(cursorType));
		}
	}
}
