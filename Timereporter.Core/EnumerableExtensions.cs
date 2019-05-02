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

		public static Workdays ToWorkdays(this List<Event> events, DateTimeZone timeZone)
		{
			var reduced =
				from e in events
				group e by new { Date = new Date(DoubleExtensions.ToLocalDateTimestampMilliseconds(e.Timestamp)), e.Kind } into eg
				select new
				{
					eg.Key.Date,
					eg.Key.Kind,
					EsentArrival = eg.FirstOrDefault(e => e.Kind == "ESENT_MIN").SomeNotNull(),
					EsentDepature = eg.FirstOrDefault(e => e.Kind == "ESENT_MAX").SomeNotNull(),
					OtherArrival = eg.FirstOrDefault(e => e.Kind == "OTHEREVENT_MIN").SomeNotNull(),
					OtherDepature = eg.FirstOrDefault(e => e.Kind == "OTHEREVENT_MAX").SomeNotNull()

					// Should really be singles
				};

			Option<float> RoundHours(Option<long> unixTimestamp)
			{
				var hours = Option.None<float>();
				unixTimestamp.MatchSome(ts =>
				{
					var value = ts.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TimeOfDay;
					var roundedHours = (float) value.Hour + (float)value.Minute / (float)60 + (float)value.Second / (float)3600 + (float)value.Millisecond / (float)360000;
					hours = roundedHours.Some();
				});
				return hours;

			}
			
			IEnumerable<WorkdayDto> Rereduce()
			{
				foreach (var r in reduced)
				{
					//Workday workday = new Workday();
					//workday.DateText = r.Date.DateText;
					Option<long> arrival = Option.None<long>();
					Option<long> departure = Option.None<long>();

					arrival.MatchNone(() => r.EsentArrival.MatchSome(a => arrival = a.Timestamp.Some()));
					arrival.MatchNone(() => r.OtherArrival.MatchSome(a => arrival = a.Timestamp.Some()));
					departure.MatchNone(() => r.EsentArrival.MatchSome(a => departure = a.Timestamp.Some()));
					departure.MatchNone(() => r.OtherArrival.MatchSome(a => departure = a.Timestamp.Some()));

					yield return new WorkdayDto
					{
						Date = r.Date.DateText(),
						ArrivalHours = RoundHours(arrival).ValueOr(0),
						BreakHours = 0,
						DepartureHours = RoundHours(departure).ValueOr(0),
					};
				}
			}

			return new Workdays
			{
				List = Rereduce().ToList()
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
