using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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

	public static class EnumerableExtensions
	{
		public static List<T> ToList<T>(this IEnumerable<T> iterable, Action<int, T> tap, Predicate<T> filter = null)
		{
			List<T> list = new List<T>();
			foreach (var tuple in iterable.Select((item, i) => (Item: item, Index: i)))
			{
				tap(tuple.Index, tuple.Item);
				if (filter == null || filter(tuple.Item))
				{
					list.Add(tuple.Item);
				}
			}
			return list;
		}

		public static IEnumerable<WorkdayDto> ToWorkdays(this List<Event> events, DateTimeZone timeZone)
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
					UserArrival = ReduceTime(eg, "USER_MIN", g => g.Min()),
					UserDeparture = ReduceTime(eg, "USER_MAX", g => g.Max()),
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
					var roundedHours = (float)value.Hour + (float)value.Minute / (float)60 + (float)value.Second / (float)3600 + (float)value.Millisecond / (float)360000;
					hours = roundedHours.Some();
				});
				return hours;
			}

			IEnumerable<WorkdayDto> RereduceTime()
			{
				foreach (var r in reduceDate)
				{
					Option<(long, TimeConfidence)> arrival = Option.None<(long, TimeConfidence)>();
					Option<(long, TimeConfidence)> departure = Option.None<(long, TimeConfidence)>();

					// This is to messy for a worksheet update. Use CRUD approach for stored cell values.!!

					arrival.MatchNone(() => r.UserArrival.MatchSome(a => arrival = (a, TimeConfidence.Certain).Some()));
					arrival.MatchNone(() => r.EsentArrival.MatchSome(a => arrival = (a, TimeConfidence.Confident).Some()));
					arrival.MatchNone(() => r.OtherArrival.MatchSome(a => arrival = (a, TimeConfidence.Insecure).Some()));

					departure.MatchNone(() => r.UserDeparture.MatchSome(a => departure = (a, TimeConfidence.Certain).Some()));
					departure.MatchNone(() => r.EsentDepature.MatchSome(a => departure = (a, TimeConfidence.Confident).Some()));
					departure.MatchNone(() => r.OtherDepature.MatchSome(a => departure = (a, TimeConfidence.Certain).Some()));
					
					Option<long> total = Option.None<long>();

					// No break for now. No use of Summarize either
					arrival.MatchSome(a => departure.MatchSome(d => total = (d.Item1 - a.Item1).Some()));

					yield return new WorkdayDto
					{
						Date = r.Date.DateText(),
						ArrivalHours = RoundHours(arrival),
						ArrivalConfidence = arrival.ValueOr((0, TimeConfidence.None)).Item2,
						BreakHours = Option.None<float>(),
						DepartureHours = RoundHours(departure),
						DepartureConfidence = departure.ValueOr((0, TimeConfidence.None)).Item2
					};
				}
			}

			// System.Diagnostics.Debugger.Break();

			return RereduceTime().ToList();
		}

		public static IEnumerable<WorkdayDetailsDto> ToWorkdayDetails(this IEnumerable<WorkdayDto> workday)
		{
			foreach(var wd in workday)
			{
				yield return new WorkdayDetailsDto
				{
					Date = wd.Date,
					ArrivalHours = wd.ArrivalHours.Match(some: t => t.ToString("0.0"), () => ""),
					ArrivalConfidence = wd.ArrivalConfidence.ToString(),
					BreakHours = wd.ArrivalHours.Match(some: t => t.ToString("0.0"), () => ""),
					DepartureHours = wd.DepartureHours.Match(some: t => t.ToString("0.0"), () => ""),
					DepartureConfidence = wd.DepartureConfidence.ToString(),
					Total = Total(wd).Match(some: t => t.ToString("0.0"), () => "")
				};
			}
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

				// TODO: Replace with Some(Action<>) me thinks. Also use workday reporting instead of events. Also push events rather.
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

		public static Option<float> Total(this WorkdayDto workday)
		{
			void GetTotal(float arrival, float departure, Option<float> @break, out Option<float> t)
			{
				float total_ = departure - arrival;
				@break.MatchSome(b => total_ -= b);
				t = total_.Some();
			}

			var total = Option.None<float>();
			workday.ArrivalHours.MatchSome(a => workday.DepartureHours.MatchSome(d => GetTotal(a, d, workday.BreakHours, out total)));
			return total;
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
			for (int i = 0; year >= firstDayOfWeekOne.PlusDays(i).Year; i++)
			{
				if (i % 7 == 0)
				{
					week++;
				}

				lookup.Add(firstDayOfWeekOne.PlusDays(i), week);
			}

			return lookup;
		}

		public static long FromHourDecimalExpressionToUnixTimestampMilliseconds(this string hourDecimalExpression, LocalDate localDate, DateTimeZone tdz)
		{
			decimal hourDecimal;
			{
				if (!decimal.TryParse(hourDecimalExpression, out hourDecimal))
				{
					var style = NumberStyles.AllowDecimalPoint
						| NumberStyles.AllowThousands
						| NumberStyles.AllowTrailingWhite;

					hourDecimal = decimal.Parse(hourDecimalExpression, style, CultureInfo.InvariantCulture);
				}
			}
			var hour = Math.Floor(hourDecimal);
			var decimalOnly = hourDecimal - hour;
			int hour_ = Convert.ToInt32(hour);
			int minutes = Convert.ToInt32(decimalOnly * 60m);

			LocalDateTime localDateTime = new LocalDateTime(localDate.Year, localDate.Month, localDate.Day, hour_, minutes);
			ZonedDateTime dateTimeZoned = localDateTime.InZoneLeniently(tdz);
			Instant instant = dateTimeZoned.ToInstant();
			return instant.ToUnixTimeMilliseconds();
		}

		public static string Base64Encode(this string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(this  string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
