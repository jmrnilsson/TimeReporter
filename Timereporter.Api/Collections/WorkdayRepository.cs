using NodaTime;
using Optional;
using Optional.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timereporter.Api.Models;
using Timereporter.Core;
using Timereporter.Core.Models;
using Event = Timereporter.Core.Models.Event;

namespace Timereporter.Api.Collections
{

	public class WorkdayRepository : IWorkdayRepository
	{
		private readonly DatabaseContextFactoryDelegate databaseContextFactory;

		public WorkdayRepository(DatabaseContextFactoryDelegate databaseContextFactory)
		{
			this.databaseContextFactory = databaseContextFactory;
		}

		public List<Event> Find(Instant fromDate, Instant exclusiveToDate)
		{
			// DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

			using (DatabaseContext db = databaseContextFactory())
			{
				IQueryable<Event> events =
					from e in db.Events
					where e.Timestamp >= fromDate.ToUnixTimeMilliseconds()
					where e.Timestamp < exclusiveToDate.ToUnixTimeMilliseconds()
					select new Event(e.Kind, e.Timestamp);

				return events.ToList();
			}
		}

		public Option<WorkdayDto> FindByKey(string date)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				var query =
				(
					from wd in db.Workdays
					where wd.Date == date
					select wd
				).ToList();

				//var query0 =
					//query.Select(wd => new WorkdayDto
					//{
					//	ArrivalHours = wd.ArrivalMilliseconds.SomeNotNull().Map(m => (float) m / 60000) ,
					//	DepartureHours = wd.DepartureMilliseconds.SomeNotNull().Map(d => (float)d / 60000),
					//	BreakHours = wd.BreakMilliseconds.SomeNotNull().Map(b => (float)b/ 60000),
					//	Date = wd.Date,
					//	Changed = wd.Changed.ToUnixDateTimeMilliseconds()
					//});
				return Option.None<WorkdayDto>();
				// return query0.SingleOrNone();
			}
		}

		public void Save(WorkdayDto value)
		{
			var instant = SystemClock.Instance.GetCurrentInstant();
			DateTime now = instant.ToDateTimeUtc();

			WorkdayDo Create(DatabaseContext db)
			{
				var wd = new WorkdayDo() { Added = now };
				db.Add(wd);
				return wd;
			}

			//using (DatabaseContext db = databaseContextFactory())
			//{
			//	var option = db.Workdays.SingleOrNone(c => c.Date == value.Date);
			//	var model = option.ValueOr(() => Create(db));
			//	model.Changed = now;
			//	value.ArrivalHours.Match(some: a => model.ArrivalMilliseconds = (int)a * 60000, none: () => model.ArrivalMilliseconds = null);
			//	value.BreakHours.Match(some: b => model.BreakMilliseconds = (int)b * 60000, none: () => model.BreakMilliseconds = null);
			//	value.DepartureHours.Match(some: b => model.DepartureMilliseconds = (int)b * 60000, none: () => model.DepartureMilliseconds = null);
			//	db.SaveChanges();
			//}
		}
	}
}
