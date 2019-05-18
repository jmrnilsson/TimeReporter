using NodaTime;
using Optional;
using Optional.Collections;
using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Timereporter.Core.Collections;
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
		
		public static IEnumerable<WorkdayDetailsDto> ToWorkdayDetails(this IEnumerable<WorkdayDto> workday)
		{
			foreach(var wd in workday)
			{
				yield return new WorkdayDetailsDto
				{
					Date = wd.Date,
					ArrivalHours = wd.ArrivalHours.Match(some: t => t.ToString("0.0"), none: () => ""),
					ArrivalConfidence = wd.ArrivalConfidence.ToString(),
					BreakHours = wd.BreakHours.Match(some: t => t.ToString("0.0"), none: () => ""),
					DepartureHours = wd.DepartureHours.Match(some: t => t.ToString("0.0"), none: () => ""),
					DepartureConfidence = wd.DepartureConfidence.ToString(),
					Total = Total(wd).Match(some: t => t.ToString("0.0"), none: () => "")
				};
			}
		}

		public static List<Event> ToList(this Dictionary<string, Time> times)
		{
			List<Event> events = new List<Event>();

			foreach (var t in times)
			{
				// TODO: Replace with Some(Action<>) me thinks. Also use workday reporting instead of events. Also push events rather.
				t.Value.Source.MatchSome(source =>
				{
					t.Value.Min.MatchSome(some: min => events.Add(new Event($"{source}_MIN", min)));
					t.Value.Max.MatchSome(some: max => events.Add(new Event($"{source}_MAX", max)));
				});
			}
			return events;
		}

		public static IEnumerable<List<Event>> Chunkmap(this IEnumerable<Event> entries)
		{
			List<Event> events = new List<Event>();

			foreach (var t in entries)
			{
				if (events.Count > 99)
				{
					yield return events;
					events = new List<Event>();
				}

				// TODO: Replace with Some(Action<>) me thinks. Also use workday reporting instead of events. Also push events rather.
				events.Add(t);
			}
			yield return events;
		}

		public static IEnumerable<Event> MapToEvents(this IEnumerable<IEventLogEntryProxy> entries, DateTimeZone dtz)
		{
			return entries.Select(e => new Event(e.Source, EnumerableExtensions.ToInstantFromLocal(e.TimeWritten, dtz, true)));
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
					yield return new Workday(new LocalDate(date.Year, date.Month, date.Day), 0, 0, 0);
				}
			}

			return EnumerateWorkdays_().ToArray();
		}

		public static LocalDate[] DateRange(LocalDate from, LocalDate to)
		{
			IEnumerable<LocalDate> EnumerateDates_()
			{
				for (int i = 0; from.PlusDays(i) < to.PlusDays(1); i++)
				{
					yield return from.PlusDays(i);
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

		

		public static bool IsWeekend(this LocalDate localDate)
		{
			return localDate.DayOfWeek == IsoDayOfWeek.Sunday
				|| localDate.DayOfWeek == IsoDayOfWeek.Saturday
				|| OfficialHolidays.List.Any(oh => oh.Equals(localDate));
		}

		public static Instant ToInstantFromLocal(DateTime dt, DateTimeZone timeZone, bool assert = false)
		{
			bool LocalDateTimeEquals(DateTime x, LocalDateTime y)
			{
				return x.Year == y.Year
					&& x.Month == y.Month
					&& x.Day == y.Day
					&& x.Second == y.Second
					&& x.Millisecond == y.Millisecond;
			}

			LocalDateTime localDateTime = new LocalDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
			ZonedDateTime dateTimeZoned = localDateTime.InZoneLeniently(timeZone);
			Instant instant = dateTimeZoned.ToInstant();

			if (assert)
			{
				var dateTimeZoned_ = instant.InZone(timeZone);
				LocalDateTime localDateTime_ = dateTimeZoned_.LocalDateTime;
				if (!LocalDateTimeEquals(dt, localDateTime_))
				{
					throw new ArgumentException($"{nameof(ToInstantFromLocal)}: Reverse datetime-conversion test failed.");
				}
			}

			return instant;
		}

	}
}
