using System;
using System.Linq;
using Timereporter.Api.Models;
using Timereporter.Api.Collections.Queries;
using Event = Timereporter.Core.Entities.Event;
using System.Collections.Generic;
using Timereporter.Api.Collections.Interfaces;

namespace Timereporter.Api.Collections
{
	public interface IEvents : IPersistentLog<YearMonth, Date, Event> { }

	public class Events : IEvents
	{
		private readonly DatabaseContextFactoryDelegate databaseContextFactory;

		public Events(DatabaseContextFactoryDelegate databaseContextFactory)
		{
			this.databaseContextFactory = databaseContextFactory;
		}

		private static void Add(DatabaseContext db, Event @event)
		{
			var model = new Models.Event();
			model.Added = @event.Added;
			model.Kind = @event.Kind;
			model.Moment = @event.Moment;
			db.Events.Add(model);
		}

		public void Add(Event @event)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				Add(db, @event);
				db.SaveChanges();
			}
		}

		public void Add(IEnumerable<Event> events)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				foreach(var e in events)
				{
					Add(db, e);
				}
				db.SaveChanges();
			}
		}

		public Event[] FindBy(Date query)
		{
			var start = new DateTime(query.Year, query.Month, query.Day);
			var exclusiveEnd = start.AddDays(1);
			return FindBy(start, exclusiveEnd);
		}

		public Event[] FindBy(YearMonth query)
		{
			int year, month;
			year = query.Year;
			month = query.Month;
			var start = new DateTime(year, month, 1);
			var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
			var exclusiveEnd = end.AddDays(1);
			return FindBy(start, exclusiveEnd);
		}

		private Event[] FindBy(DateTime start, DateTime exclusiveEnd)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				var query =
					from e in db.Events
					where e.Moment >= start
					where e.Moment < exclusiveEnd
					select new Event(e.Added, e.Kind, e.Moment);

				return query.ToArray();
			}
		}
	}
}
