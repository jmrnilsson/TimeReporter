using NodaTime;
using Optional;
using Optional.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Api.Models;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.Api.Collections
{
	// Most of these things should probably be move to some richer object!
	public class WorkdayRepository : IWorkdayRepository
	{
		private readonly DatabaseContextFactoryDelegate databaseContextFactory;

		public WorkdayRepository(DatabaseContextFactoryDelegate databaseContextFactory)
		{
			this.databaseContextFactory = databaseContextFactory;
		}

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

		public Option<WorkdaySlice> Find(int date)
		{
			return Find_(date).Match(some: wd => wd.Some(), none: () => Option.None<WorkdaySlice>());
		}

		private Option<WorkdaySlice> Find_(int date)
		{
			using (DatabaseContext db = databaseContextFactory())
			{
				var query =
				(
					from wd in db.Workdays
					where wd.Date == date
					select new
					{
						Date = new DateText(wd.Date).ToString(),
						wd.Departure,
						wd.Arrival,
						wd.Break,
						wd.HashCode,

					}
				).ToList();

				var query0 = query.Select(wd => new WorkdaySlice
				{
					Arrival = wd.Arrival.SomeNotNull().Match(a => a.Value.Some(), () => Option.None<long>()),
					Departure = wd.Departure.SomeNotNull().Match(a => a.Value.Some(), () => Option.None<long>()),
					Break = wd.Break.SomeNotNull().Match(a => a.Value.Some(), () => Option.None<long>()),
					Date = wd.Date,
					HashCode = wd.HashCode
				});

				return Option.None<WorkdaySlice>();
			}
		}

		public void Save(IWorkdaySlice slice)
		{
			Save_(new List<IWorkdaySlice>() { slice });
		}

		public void Save(List<IWorkdaySlice> slices)
		{
			Save_(slices);
		}


		private void Save_(List<IWorkdaySlice> slices)
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
					var sliceOption = db.Workdays.SingleOrNone(c => c.Date == new DateText(slice.Date).ToInt32() && c.Kind == slice.Kind);
					bool unchanged = sliceOption.Match(o => o.HashCode == slice.HashCode, () => false);

					if (unchanged)
					{
						continue;
					}

					var model = sliceOption.ValueOr(() => Create(db, slice.Date, slice.Kind));
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
