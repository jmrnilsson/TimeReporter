using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.Core
{
	public static class Extensions
	{
		public static List<T> ToList<T>(this IEnumerable<T> iterable, Action<int, T> tap, Predicate<T> filter = null)
		{
			List<T> list = new List<T>();
			foreach (var tuple in iterable.Select((item, i) => (Item: item, Index:i)))
			{
				tap(tuple.Index, tuple.Item);
				if (filter == null || filter(tuple.Item))
				{
					list.Add(tuple.Item);
				}
			}
			return list;
		}

		public static Workdays ToWorkdays(this DateTimeZone timeZone, IQueryable<Event> events)
		{
			return new Workdays
			{
				List =
				(
					from e in events
					group e by new Date(DoubleExtensions.ToLocalDateTimestampMilliseconds(e.Timestamp)) into eg
					let min = eg.FirstOrDefault(e => e.Kind == "esent_min")
					let min_ = min.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TimeOfDay
					let max = eg.FirstOrDefault(e => e.Kind == "esent_max")
					let max_ = max.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TimeOfDay
					select new WorkdayDto
					{
						Date = eg.Key.DateText(),
						ArrivalHours = min != null ? (float)min_.Hour + (float)min_.Minute / (float)60 + (float)min_.Second / (float)360 : 0,
						BreakHours = 0,
						DepartureHours = max != null ? (float)max_.Hour + (float)max_.Minute / (float)60 + (float)max_.Second / (float)360 : 0
					}
				).ToList()
			};
		}

		public static IEnumerable<List<Event>> Chunkmap(this Dictionary<string, Time> times)
		{
			List<Event> events = new List<Event>();

			foreach (var t in times)
			{
				if (events.Count > 998)
				{
					yield return events;
					events.Clear();
				}

				// TODO: Replace with Some(Action<>) me thinks
				t.Value.Source.MatchSome(source =>
				{
					t.Value.Min.MatchSome(some: min => events.Add(new Event($"{source}_MIN", min)));
					t.Value.Max.MatchSome(some: max => events.Add(new Event($"{source}_MAX", max)));
				});
			}
			yield return events;
		}


		public static IWorkday[] WorkdayRange(int year, int month)
		{
			DateTime start, end;
			{
				var daysInMonth = DateTime.DaysInMonth(year, month);
				start = new DateTime(year, month, 1);
				end = new DateTime(year, month, daysInMonth);
			}

			IEnumerable<Workday> EnumerateWorkdays_()
			{
				for (int i = 0; start.AddDays(i) < end.AddDays(1); i++)
				{
					var date = start.AddDays(i);
					yield return new Workday(new Date(date), 0, 0, 0);
				}
			}

			return EnumerateWorkdays_().ToArray();
		}

		public static Date[] DateRange(Date from, Date to)
		{
			IEnumerable<Date> EnumerateDates_()
			{
				for (int i = 0; from.With(i) < to.With(1); i++)
				{
					var date = from.With(i);
					yield return new Date(date);
				}
			}

			return EnumerateDates_().ToArray();
		}
	}
}
