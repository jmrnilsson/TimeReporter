using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Api.Models;
using Event = Timereporter.Api.Entities.Event;

namespace Timereporter.Api.Collections
{
	public class Events
	{
		public void Add(Event @event)
		{
			using (DatabaseContext db = Startup.CreateDb())
			{
				var model = new Models.Event();
				model.Added = @event.Added;
				model.Kind = @event.Kind;
				model.Moment = @event.Moment;
				db.Events.Add(model);
				db.SaveChanges();
			}
		}

		public List<Event> FindByMoment(int year, int month, int day)
		{
			var start = new DateTime(year, month, day);
			var exclusiveEnd = start.AddDays(1);
			return FindByMoment(start, exclusiveEnd);
		}

		public List<Event> FindByMoment(int year, int month)
		{
			var start = new DateTime(year, month, 1);
			var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
			var exclusiveEnd = end.AddDays(1);
			return FindByMoment(start, exclusiveEnd);
		}

		private List<Event> FindByMoment(DateTime start, DateTime exclusiveEnd)
		{
			using (DatabaseContext db = Startup.CreateDb())
			{
				return
				(
					from e in db.Events
					where e.Moment >= start
					where e.Moment < exclusiveEnd
					select new Event(e.Added, e.Kind, e.Moment)
				).ToList();
			}
		}
	}
}
