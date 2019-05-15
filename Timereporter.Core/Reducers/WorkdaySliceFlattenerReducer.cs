using NodaTime;
using Optional;
using Optional.Collections;
using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;

namespace Timereporter.Core.Reducers
{
	public static class WorkdaySliceFlattenerReducer
	{

		public static IEnumerable<WorkdayDto> ToWorkdays(List<IWorkdaySlice> workdays, DateTimeZone timeZone)
		{
			Option<long> SelectOrNone(IGrouping<string, IWorkdaySlice> grouping, string kind, Func<IWorkdaySlice, Option<long>> valueFactory)
			{
				return
				(
					from wds in grouping
					where wds.Kind == kind
					let value = valueFactory(wds)
					where value.HasValue
					select value.ValueOrFailure()
				).SingleOrNone();
			}

			var reduceDate =
				from e in workdays
				group e by e.Date into eg
				select new
				{
					Date = eg.Key,
					EsentArrival = SelectOrNone(eg, "ESENT", e => e.Arrival),
					EsentBreak = SelectOrNone(eg, "ESENT", e => e.Break),
					EsentDepature = SelectOrNone(eg, "ESENT", e => e.Departure),
					UserArrival = SelectOrNone(eg, "USER", e => e.Arrival),
					UserBreak = SelectOrNone(eg, "USER", e => e.Break),
					UserDepature = SelectOrNone(eg, "USER", e => e.Departure)
				};

			Option<float> RoundHours(Option<(long, TimeConfidence)> millisecondOfday)
			{
				var hours = Option.None<float>();
				millisecondOfday.MatchSome(msOfDay =>
				{
					var roundedHours = msOfDay.Item1 / (float) 1000 / 60 / 60;
					// var value = ts.Item1.ToInstantFromUnixTimestampMilliseconds().InZone(timeZone).LocalDateTime.TimeOfDay;
					//var roundedHours = (float)value.Hour + (float)value.Minute / (float)60 + (float)value.Second / (float)3600 + (float)value.Millisecond / (float)360000;
					//var roundedHours = (float)value.Hour + (float)value.Minute / (float)60 + (float)value.Second / (float)3600 + (float)value.Millisecond / (float)360000;
					hours = roundedHours.Some();
				});
				return hours;
			}

			IEnumerable<WorkdayDto> RereduceTime()
			{
				foreach (var r in reduceDate)
				{
					Option<(long, TimeConfidence)> arrival = Option.None<(long, TimeConfidence)>();
					Option<(long, TimeConfidence)> @break = Option.None<(long, TimeConfidence)>();
					Option<(long, TimeConfidence)> departure = Option.None<(long, TimeConfidence)>();

					// This is to messy for a worksheet update. Use CRUD approach for stored cell values.!!

					arrival.MatchNone(() => r.UserArrival.MatchSome(a => arrival = (a, TimeConfidence.Certain).Some()));
					arrival.MatchNone(() => r.EsentArrival.MatchSome(a => arrival = (a, TimeConfidence.Confident).Some()));

					@break.MatchNone(() => r.UserBreak.MatchSome(a => arrival = (a, TimeConfidence.Certain).Some()));
					@break.MatchNone(() => r.EsentBreak.MatchSome(a => arrival = (a, TimeConfidence.Confident).Some()));

					departure.MatchNone(() => r.UserArrival.MatchSome(a => departure = (a, TimeConfidence.Certain).Some()));
					departure.MatchNone(() => r.EsentDepature.MatchSome(a => departure = (a, TimeConfidence.Confident).Some()));

					Option<long> total = Option.None<long>();

					// No break for now. No use of Summarize either
					arrival.MatchSome(a => departure.MatchSome(d => total = (d.Item1 - a.Item1).Some()));

					yield return new WorkdayDto
					{
						Date = r.Date,
						ArrivalHours = RoundHours(arrival),
						ArrivalConfidence = arrival.Match(some => some.Item2, () => TimeConfidence.None),
						BreakHours = RoundHours(@break),
						DepartureHours = RoundHours(departure),
						DepartureConfidence = departure.Match(some => some.Item2, () => TimeConfidence.None)
					};
				}
			}

			return RereduceTime().ToList();
		}
	}
}
