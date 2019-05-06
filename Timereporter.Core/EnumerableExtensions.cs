using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.Core
{
	public enum TimeConfidence : short
	{
		Certain = 1,
		Confident,
		Insecure,
		None
	}

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

		public static Workdays ToWorkdays(this List<Event> events, DateTimeZone timeZone)
		{
			Option<long> ReduceTime(IGrouping<Date, Event> grouping, string kind, Func<IEnumerable<long>, long> accumulator)
			{
				Option<long> time = Option.None<long>();

				grouping
					.Where(g => g.Kind == kind)
					.Select(g => g.Timestamp)
					.SomeWhen(e => e.Any())
					.MatchSome(some: g => time = accumulator(g).Some());

				return time;
			}

			var reduceDate =
				from e in events
				group e by new Date(DoubleExtensions.ToLocalDateTimestampMilliseconds(e.Timestamp)) into eg
				select new
				{
					Date = eg.Key,
					EsentArrival = ReduceTime(eg, "ESENT_MIN", g => g.Min()),
					EsentDepature = ReduceTime(eg, "ESENT_MAX", g => g.Max()),
					OtherArrival = ReduceTime(eg, "OTHEREVENT_MIN", g => g.Min()),
					OtherDepature = ReduceTime(eg, "OTHEREVENT_MAX", g => g.Max()),

					// Because logged times do not use date as key b

					//EsentA = eg.Where(e => e.Kind == "ESENT_MIN").ToList(),
					//OtherA = eg.Where(e => e.Kind == "ESENT_MAX").ToList(),
					//EsentD = eg.Where(e => e.Kind == "OTHEREVENT_MIN").ToList(),
					//OtherD = eg.Where(e => e.Kind == "OTHEREVENT_MAX").ToList()

					// Should really be partitioned by localDate of the clients timezone.
				};

			Option<float> RoundHours(Option<(long, TimeConfidence)> unixTimestamp)
			{
				var hours = Option.None<float>();
				unixTimestamp.MatchSome(ts =>
				{
					var value = ts.Item1.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TimeOfDay;
					var roundedHours = (float) value.Hour + (float)value.Minute / (float)60 + (float)value.Second / (float)3600 + (float)value.Millisecond / (float)360000;
					hours = roundedHours.Some();
				});
				return hours;
			}

			string ConfidenceText(Option<(long, TimeConfidence)> unixTimestamp)
			{
				return unixTimestamp.ValueOr((0, TimeConfidence.None)).Item2.ToString();
			}
			
			IEnumerable<WorkdayDto> RereduceTime()
			{
				foreach (var r in reduceDate)
				{
					Option<(long, TimeConfidence)> arrival = Option.None<(long, TimeConfidence)>();
					Option<(long, TimeConfidence)> departure = Option.None<(long, TimeConfidence)>();

					arrival.MatchNone(() => r.EsentArrival.MatchSome(a => arrival = (a, TimeConfidence.Confident) .Some()));
					arrival.MatchNone(() => r.OtherArrival.MatchSome(a => arrival = (a, TimeConfidence.Insecure).Some()));
					departure.MatchNone(() => r.EsentDepature.MatchSome(a => departure = (a, TimeConfidence.Confident).Some()));
					departure.MatchNone(() => r.OtherDepature.MatchSome(a => departure = (a, TimeConfidence.Certain).Some()));

					yield return new WorkdayDto
					{
						Date = r.Date.DateText(),
						ArrivalHours = RoundHours(arrival).ValueOr(0),
						ArrivalConfidence = ConfidenceText(arrival),
						BreakHours = 0,
						DepartureHours = RoundHours(departure).ValueOr(0),
						DepartureConfidence = ConfidenceText(departure)
					};
				}
			}

			// System.Diagnostics.Debugger.Break();

			return new Workdays
			{
				List = RereduceTime().ToList()
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

		public static IEnumerable<LocalDate> ReverseMonthRange(Instant instant, DateTimeZone tz, int count)
		{
			LocalDate today = instant.InZone(tz).Date;
			int year = today.Year;
			int month = today.Month;

			for (int i = 0; i < count; i++)
			{
				var localDate = new LocalDate(year, month, 1).Minus(Period.FromDays(i));
				year = localDate.Year;
				month = localDate.Month;
				yield return localDate;
			}
		}

		public static float SummarizeWorkday(this WorkdayDto workday)
		{
			return workday.DepartureHours - workday.BreakHours - workday.ArrivalHours;
		}

		public static Dictionary<LocalDate, int> GetEuropeanWeeks(this int year)
		{
			var firstDay = new LocalDate(year, 1, 1);

			// TODO: Inline-method switch this instead;
			LocalDate firstDayOfWeekOne;
			if (firstDay.DayOfWeek == IsoDayOfWeek.Monday)
			{
				firstDayOfWeekOne = firstDay;
			}
			else if (firstDay.DayOfWeek == IsoDayOfWeek.Tuesday)
			{
				firstDayOfWeekOne = firstDay.Minus(Period.FromDays(1));
			}
			else if (firstDay.DayOfWeek == IsoDayOfWeek.Wednesday)
			{
				firstDayOfWeekOne = firstDay.Minus(Period.FromDays(2));
			}
			else if (firstDay.DayOfWeek == IsoDayOfWeek.Thursday)
			{
				firstDayOfWeekOne = firstDay.Minus(Period.FromDays(3));
			}
			else if (firstDay.DayOfWeek == IsoDayOfWeek.Friday)
			{
				firstDayOfWeekOne = firstDay.PlusDays(3);
			}
			else if (firstDay.DayOfWeek == IsoDayOfWeek.Saturday)
			{
				firstDayOfWeekOne = firstDay.PlusDays(2);
			}
			else if (firstDay.DayOfWeek == IsoDayOfWeek.Sunday)
			{
				firstDayOfWeekOne = firstDay.PlusDays(1);
			}
			else
			{
				throw new DataMisalignedException("Week numbers make no sense");
			}

			Dictionary<LocalDate, int> lookup = new Dictionary<LocalDate, int>();

			int week = 0;
			for(int i = 0; year >= firstDayOfWeekOne.PlusDays(i).Year; i++)
			{
				if (i % 7 == 0)
				{
					week++;
				}

				lookup.Add(firstDayOfWeekOne.PlusDays(i), week);
			}

			return lookup;
		}
	}
}
