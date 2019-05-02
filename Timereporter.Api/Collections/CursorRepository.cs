using System;
using System.Linq;
using Optional;
using Timereporter.Api.Models;
using Cursor = Timereporter.Core.Models.Cursor;

namespace Timereporter.Api.Collections
{

	public class CursorRepository : ICursorRepository
	{
		private readonly DatabaseContextFactoryDelegate dataContextFactory;

		public CursorRepository(DatabaseContextFactoryDelegate dataContextFactory)
		{
			this.dataContextFactory = dataContextFactory;
		}

		public void Save(Cursor cursor)
		{
			Models.CursorDo Create(DatabaseContext db)
			{
				var c = new Models.CursorDo() { Added = cursor.Changed };
				db.Add(c);
				return c;
			}

			using (DatabaseContext db = dataContextFactory())
			{
				var option = db.Cursors.SingleOrDefault(c => c.CursorType == cursor.CursorType).SomeNotNull();
				var model = option.ValueOr(() => Create(db));
				model.Changed = cursor.Changed;
				model.CursorType = cursor.CursorType;
				model.Position = cursor.Position;
				db.SaveChanges();
			}
		}

		public Option<Cursor> FindByKey(string cursorType)
		{
			using (DatabaseContext db = dataContextFactory())
			{
				var query =
					from e in db.Cursors
					where e.CursorType == cursorType
					select new Cursor(e.Changed, e.CursorType, e.Position);

				return query.SingleOrDefault().SomeNotNull();
			}
		}
	}
}
