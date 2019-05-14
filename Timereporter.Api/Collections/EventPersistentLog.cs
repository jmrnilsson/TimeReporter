using System;
using System.Linq;
using Timereporter.Api.Models;
using Event = Timereporter.Core.Models.Event;
using System.Collections.Generic;
using NodaTime;
using Optional;

namespace Timereporter.Api.Collections
{
	public class EventPersistentLog : IEventPersistentLog
	{
		private readonly DatabaseContextFactoryDelegate databaseContextFactory;

		public EventPersistentLog(DatabaseContextFactoryDelegate databaseContextFactory)
		{
			this.databaseContextFactory = databaseContextFactory;
		}

		public void AddRange(IEnumerable<Event> events)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				foreach(var e in events)
				{
					var option = db.Events.SingleOrDefault(c => c.Kind == e.Kind && c.Timestamp == e.Timestamp).SomeNotNull();
					var model = option.ValueOr(() => Make(db, e.Kind, e.Timestamp));
					model.Changed = DateTime.UtcNow;
				};

				db.SaveChanges();
			}
		}

		private static EventDo Make(DatabaseContext db, string kind, long timestamp)
		{
			var c = new Models.EventDo()
			{
				Added = DateTime.UtcNow,
				Changed = DateTime.UtcNow,
				Kind = kind,  // Because composite keys, dunno
				Timestamp = timestamp
			};
			db.Add(c);
			return c;
		}

		public Event[] FindBy((Instant, Instant) args)
		{
			return FindBy((args.Item1.ToUnixTimeMilliseconds(), args.Item2.ToUnixTimeMilliseconds()));
		}

		public Event[] FindBy((long, long) args)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				var query =
					from e in db.Events
					where e.Timestamp >= args.Item1
					where e.Timestamp < args.Item2
					select new Event(e.Kind, e.Timestamp);

				return query.ToArray();
			}
		}
	}
}
