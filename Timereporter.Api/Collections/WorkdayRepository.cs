﻿using NodaTime;
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

		//public List<Event> Find(Instant fromDate, Instant exclusiveToDate)
		//{
		//	using (DatabaseContext db = databaseContextFactory())
		//	{
		//		IQueryable<Event> events =
		//			from e in db.Events
		//			where e.Timestamp >= fromDate.ToUnixTimeMilliseconds()
		//			where e.Timestamp < exclusiveToDate.ToUnixTimeMilliseconds()
		//			select new Event(e.Kind, e.Timestamp);

		//		return events.ToList();
		//	}
		//}

		public List<IWorkdaySlice> Find(LocalDate fromDate, LocalDate exclusiveToDate)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				var workdaySlices =
				(
					from e in db.Workdays
					where e.Date >= new DateText(fromDate).ToInt32()
					where e.Date < new DateText(exclusiveToDate).ToInt32()
					select new
					{
						Date = new DateText(e.Date).ToString(),
						e.Kind,
						e.Arrival,
						e.Break,
						e.Departure,
					}
				).ToList();

				IEnumerable<IWorkdaySlice> workdaySliceList =
					from e in workdaySlices
					select new WorkdaySlice
					{
						Date = e.Date,
						Kind = e.Kind,
						Arrival = e.Arrival.SomeNotNull().Match(some: s => s.Value.Some(), () => Option.None<long>()),
						Break = e.Break.SomeNotNull().Match(some: s => s.Value.Some(), () => Option.None<long>()),
						Departure = e.Departure.SomeNotNull().Match(some: s => s.Value.Some(), () => Option.None<long>()),
					};

				return workdaySliceList.ToList();
			}
		}

		public Option<WorkdayDto> Find(int date)
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
			}
		}

		public void Save(List<IWorkdaySlice> slices)
		{
			var instant = SystemClock.Instance.GetCurrentInstant();
			DateTime now = instant.ToDateTimeUtc();

			WorkdayDo Create(DatabaseContext db, string date, string kind)
			{
				var wd = new WorkdayDo()
				{
					Added = now,
					Date = new DateText(date).ToInt32(),
					Kind = kind,
				};
				db.Add(wd);
				return wd;
			}

			using (DatabaseContext db = databaseContextFactory())
			{
				foreach (var slice in slices)
				{
					var option = db.Workdays.SingleOrNone(c => c.Date == new DateText(slice.Date).ToInt32() && c.Kind == slice.Kind);
					var unchanged = option.Match(o => o.HashCode == slice.HashCode, () => false);

					if (unchanged)
					{
						continue;
					}

					var model = option.ValueOr(() => Create(db, slice.Date, slice.Kind));
					model.Changed = now;
					model.Arrival = slice.Arrival.Match(a => a, () => (long?)null);
					model.Break = slice.Break.Match(b => b, () => (long?)null);
					model.Departure = slice.Departure.Match(d => d, () => (long?)null);
					model.HashCode = slice.HashCode;
				}
				db.SaveChanges();
			}
		}
	}
}
