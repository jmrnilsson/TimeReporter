using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.Core
{

	public class WorkdayCalculator
	{
		public IEnumerable<IWorkdaySlice> CalculateWorkdays(IEnumerable<Event> events, DateTimeZone timeZone)
		{
			long MillisecondOfDay(long unixTimestamp)
			{
				return unixTimestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TickOfDay / 10000;
			}

			var q =
			(
				from e in events
				group e by new { e.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).Date, e.Kind } into eg
				let timestamps = eg.Select(e => e.Timestamp)
				where new DateText(eg.Key.Date).ToString().Contains("2018-10-02")
				select new
				{
					eg.Key.Date,
					eg.Key.Kind,
					timestamps,
					Arrival = timestamps.Min(),
					Departure = timestamps.Max(),
					ArrivalMs = MillisecondOfDay(timestamps.Min()),
					DepartureMs = MillisecondOfDay(timestamps.Max()),
				}
			).ToList();

			var reduceDate =
				from e in events
				group e by new { e.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).Date, e.Kind } into eg
				let timestamps = eg.Select(e => e.Timestamp)
				let date = new DateText(eg.Key.Date).ToString()
				let arrival = MillisecondOfDay(timestamps.Min())
				let departure = MillisecondOfDay(timestamps.Max())
				orderby eg.Key.Date, eg.Key.Kind
				select new WorkdaySlice
				{
					Date = date,
					Kind = eg.Key.Kind,
					Arrival = arrival.Some(),
					Departure = departure.Some(),
					Break = Option.None<long>(),
					HashCode = $"{date}:{eg.Key.Kind}:{arrival}::{departure}".ToFnv1aHashInt32()
		};

			return reduceDate;
		}

	}
}
