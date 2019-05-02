using NodaTime;
using NodaTime.Extensions;
using Optional;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Timereporter.Core.Models
{
	public class Time
	{
		private readonly Date date;
		private readonly Option<Instant> min;
		private readonly Option<Instant> max;

		public Option<Instant> Min => date.IsWeekend() ? Option.None<Instant>() : min;
		public Option<Instant> Max => date.IsWeekend() ? Option.None<Instant>() : max;
		public string Date => date.ToString();
		public string DayOfWeek => date.DayOfWeek();

		public Time(Date date, DateTime min, DateTime max, DateTimeZone timeZone = null)
		{
			this.date = date;
			if (timeZone != null)
			{
				this.min = ToInstantFromLocal(min, timeZone, assert: true).Some();
				this.max = ToInstantFromLocal(max, timeZone, assert: true).Some();
			}
			else
			{
				try
				{
					this.min = min.ToInstant().Some();
					this.max = max.ToInstant().Some();
				}
				catch (ArgumentException e) when (Regex.IsMatch(e.Message, @"^Invalid DateTime\.Kind"))
				{
					throw;
				}
			}
		}

		public Time(Date date, Option<Instant> min, Option<Instant> max)
		{
			this.date = date;
			this.min = min;
			this.max = max;
		}

		private Instant ToInstantFromLocal(DateTime dt, DateTimeZone timeZone, bool assert = false)
		{
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

		public bool LocalDateTimeEquals(DateTime x, LocalDateTime y)
		{
			return x.Year == y.Year
				&& x.Month == y.Month
				&& x.Day == y.Day
				&& x.Second == y.Second
				&& x.Millisecond == y.Millisecond;
		}
	}
}
