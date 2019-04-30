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
		private const int limitAt = -21;
		private const int reportProgressBy = 1000;
		private readonly IDateTimeValueFactory dateTimeValueFactory;
		private readonly IEventLogProxy eventLog;
		public event ProgressChanged OnProgressChanged;
		
		public EventLogTracker(IDateTimeValueFactory dateTimeValueFactory, IEventLogProxy eventLog)
		{
			this.dateTimeValueFactory = dateTimeValueFactory;
			this.eventLog = eventLog;
		}

		public MinMaxes FindBy(EventLogQuery query)
		{
			eventLog.Log = query.LogName;

			var entries = eventLog.Entries.ToListAndTap(ReportProgress, e => Regex.IsMatch(e.Source, query.Pattern));
			var summary = Summarize(entries, query.From, query.To, query.Pattern);

			if (query.Fill)
			{
				summary = Fill(summary, query.From, query.To);
			}

			return new MinMaxes(summary);
		}

		private void ReportProgress(int i, IEventLogEntryProxy a)
		{
			if (OnProgressChanged != null && i % reportProgressBy == 0)
			{
				OnProgressChanged();
			}
		}

		private IEnumerable<MinMax> Summarize(List<IEventLogEntryProxy> entries, Date from, Date toDate, string pattern)
		{
			return
				from e in entries
				where Regex.IsMatch(e.Source, pattern)
				orderby e.TimeWritten ascending
				group e by new Date(e.TimeWritten) into eg
				where !eg.Key.IsWeekend()
				where eg.Key >= @from
				where eg.Key <= toDate
				select new MinMax
				(
					eg.Key,
					eg.Min(e => e.TimeWritten),
					eg.Max(e => e.TimeWritten)
				);
		}

		IEnumerable<MinMax> Fill(IEnumerable<MinMax> minMaxes, Date from, Date to)
		{
			var kvp = minMaxes.ToDictionary(mm => mm.Date, mm => mm);

			foreach (Date date in Workdays.EnumerateDates(from, to))
			{
				yield return kvp.GetValueOrDefault(date.DateText(), new MinMax(date, Option.None<DateTime>(), Option.None<DateTime>()));
			}
		}
	}
}
