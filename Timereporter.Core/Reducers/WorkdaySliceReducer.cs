using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core.Models;

namespace Timereporter.Core.Reducers
{
	public static class WorkdaySliceReducer
	{
		private static long MillisecondOfDay(long unixTimestamp, DateTimeZone timeZone)
		{
			return unixTimestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TickOfDay / 10000;
		}

		public static IEnumerable<IWorkdaySlice> ToWorkdaySlices(this List<Event> events, DateTimeZone timeZone)
		{
			var reduceDate =
				from e in events
				group e by new { e.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).Date, e.Kind } into eg
				let timestamps = eg.Select(e => e.Timestamp)
				let date = new DateText(eg.Key.Date).ToString()
				let arrival = MillisecondOfDay(timestamps.Min(), timeZone)
				let departure = MillisecondOfDay(timestamps.Max(), timeZone)
				orderby eg.Key.Date, eg.Key.Kind
				select new WorkdaySlice
				{
					Date = date,
					Kind = eg.Key.Kind,
					Arrival = arrival.Some(),
					Departure = departure.Some(),
					Break = Option.None<long>()
				};

			return reduceDate;
		}

	}
}
