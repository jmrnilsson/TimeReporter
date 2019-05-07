using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Timereporter.Core;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;
using Timereporter.EventLogTask.Proxies;

namespace Timereporter.EventLogTask
{
	public delegate void ProgressChanged();

	public class EventLogTracker
	{
		private const int reportProgressBy = 1000;
		private readonly IEventLogProxy eventLog;
		public event ProgressChanged OnProgressChanged;
		
		public EventLogTracker(IEventLogProxy eventLog)
		{
			this.eventLog = eventLog;
		}

		public Dictionary<string, Time> FindBy(EventLogQuery query)
		{
			eventLog.Log = query.LogName;

			var entries = eventLog.Entries.ToList(ReportProgress, e => Regex.IsMatch(e.Source, query.Pattern));
			var summary = Summarize(entries, query.From, query.To, query.Pattern);

			if (query.Fill)
			{
				summary = Fill(summary, query.From, query.To);
			}

			return summary.ToDictionary(mm => mm.Date, mm => mm);
		}

		private void ReportProgress(int i, IEventLogEntryProxy e)
		{
			if (OnProgressChanged != null && i % reportProgressBy == 0)
			{
				OnProgressChanged();
			}
		}

		private IEnumerable<Time> Summarize(List<IEventLogEntryProxy> entries, Date fromDate, Date toDate, string pattern)
		{
			Instant now = SystemClock.Instance.GetCurrentInstant();
			DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

			return
				from e in entries
				where Regex.IsMatch(e.Source, pattern)
				orderby e.TimeWritten ascending
				group e by new { Date = new Date(e.TimeWritten), e.Source } into eg
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

		private IEnumerable<Time> Fill(IEnumerable<Time> minMaxList, Date from, Date to)
		{
			var kvp = minMaxList.ToDictionary(mm => mm.Date, mm => mm);

			foreach (Date date in EnumerableExtensions.DateRange(from, to))
			{
				yield return kvp.GetValueOrDefault(date.DateText(), new Time(date, Option.None<string>(), Option.None<Instant>(), Option.None<Instant>()));
			}
		}
	}
}
