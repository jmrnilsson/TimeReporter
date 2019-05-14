using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Timereporter.Core.Models;

namespace Timereporter.Core.Reducers
{
	public static class WorkdaySimplifiedReducer
	{
		public static Dictionary<string, Time> ToSummarizedWorkdays(this List<IEventLogEntryProxy> entries_, LocalDate fromDate_, LocalDate toDate_, string pattern_, bool fill)
		{
			IEnumerable<Time> Summarize(List<IEventLogEntryProxy> entries, LocalDate fromDate, LocalDate toDate, string pattern)
			{
				DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				Instant now = SystemClock.Instance.GetCurrentInstant();

				return
					from e in entries
					where Regex.IsMatch(e.Source, pattern)
					orderby e.TimeWritten ascending
					group e by new
					{
						Date = new LocalDate(e.TimeWritten.Year, e.TimeWritten.Month, e.TimeWritten.Day),
						e.Source
					} into eg
					where !eg.Key.Date.IsWeekend()
					where eg.Key.Date >= fromDate
					where eg.Key.Date <= toDate
					select new Time
					(
						eg.Key.Date,
						eg.Key.Source,
						eg.Min(e => e.TimeWritten),
						eg.Max(e => e.TimeWritten),
						tz
					);
			}

			Time ShimGetValueOrDefault(Dictionary<string, Time> collection, string key, Time @default)
			{
				if (collection.ContainsKey(key))
				{
					return collection[key];
				}
				return @default;
			}

			IEnumerable<Time> Fill(IEnumerable<Time> minMaxList, LocalDate from, LocalDate to)
			{
				Dictionary<string, Time> kvp = minMaxList.ToDictionary(mm => mm.Date, mm => mm);

				foreach (LocalDate date in EnumerableExtensions.DateRange(from, to))
				{
					yield return ShimGetValueOrDefault
					(
						kvp,
						new DateText(date).ToString(),
						new Time(date, Option.None<string>(), Option.None<Instant>(), Option.None<Instant>())
					);
				}
			}

			var summary = Summarize(entries_, fromDate_, toDate_, pattern_);

			if (fill)
			{
				summary = Fill(summary, fromDate_, toDate_);
			}

			return summary.ToDictionary(mm => mm.Date, mm => mm);

		}

	}
}
