using System;
using System.Linq;
using Timereporter.Api.Models;
using Timereporter.Api.Collections.Queries;
using Event = Timereporter.Core.Models.Event;
using System.Collections.Generic;
using Timereporter.Api.Collections.Interfaces;
using Timereporter.Core.Models;
using NodaTime;
using Optional;

namespace Timereporter.Api.Collections
{
	public interface IEvents : IPersistentLog<(Instant, Instant), (long, long), Event> { }

	public class Events : IEvents
	{
		private readonly DatabaseContextFactoryDelegate databaseContextFactory;

		public Events(DatabaseContextFactoryDelegate databaseContextFactory)
		{
			this.databaseContextFactory = databaseContextFactory;
		}

		private Models.Event MakeAndAdd(DatabaseContext db, string kind, long timestamp)
		{
			var c = new Models.Event()
			{
				Added = DateTime.UtcNow,
				Changed = DateTime.UtcNow,
				Kind = kind,  // Because composite keys, dunno
				Timestamp = timestamp
			};
			db.Add(c);
			return c;
		}

		public void AddRange(IEnumerable<Event> events)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				foreach(var e in events)
				{
					var option = db.Events.SingleOrDefault(c => c.Kind == e.Kind && c.Timestamp == e.Timestamp).SomeNotNull();
					var model = option.ValueOr(() => MakeAndAdd(db, e.Kind, e.Timestamp));
					model.Kind = e.Kind;
					model.Timestamp = e.Timestamp;
					model.Changed = DateTime.UtcNow;
				};

				db.SaveChanges();
			}
		}

		//public Event[] FindBy(Date query, DateTimeZone timeZone)
		//{
		//	var start = query.ToDateTime();
		//	var exclusiveEnd = start.AddDays(1);
		//	return FindBy(start, exclusiveEnd);
		//}

		//public Event[] FindBy(YearMonth query)
		//{
		//	int year, month;
		//	year = query.Year;
		//	month = query.Month;
		//	var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
		//	var exclusiveEnd = end.AddDays(1);
		//	return FindBy(start, exclusiveEnd);
		//}

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
					select ModelFactory.MakeEvent(e.Kind, e.Timestamp);

				return query.ToArray();
			}
		}
	}
}
