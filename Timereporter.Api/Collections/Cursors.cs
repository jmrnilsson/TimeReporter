using System;
using System.Linq;
using Timereporter.Api.Models;
using Cursor = Timereporter.Api.Entities.Cursor;

namespace Timereporter.Api.Collections
{
	public class Cursors
	{
		public void AddOrUpdate(Cursor cursor)
		{
			using (DatabaseContext db = Startup.CreateDb())
			{
				var model = db.Cursors.SingleOrDefault(c => c.CursorType == cursor.CursorType) ?? new Models.Cursor() { Added = cursor.Changed };
				model.Changed = cursor.Changed;
				model.CursorType = cursor.CursorType;
				model.Position = cursor.Position;
				db.Cursors.Add(model);
				db.SaveChanges();
			}
		}

		public Cursor GetBy(string cursorType)
		{
			using (DatabaseContext db = Startup.CreateDb())
			{
				return
				(
					from e in db.Cursors
					where e.CursorType == cursorType
					select new Cursor(e.Changed, e.CursorType, e.Position)
				).SingleOrDefault();
			}
		}
	}
}
