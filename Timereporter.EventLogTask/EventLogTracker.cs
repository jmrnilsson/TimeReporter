using Optional;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

			var events = eventLog.Entries.ToListAndTap(ReportProgress, e => Regex.IsMatch(e.Source, query.Pattern));
			return GetMinMax(events,  query.FromDate, query.Pattern);
		}

		private void ReportProgress(int i, IEventLogEntryProxy a)
		{
			if (OnProgressChanged != null && i % reportProgressBy == 0)
			{
				OnProgressChanged();
			}
		}

		private MinMaxes GetMinMax(List<IEventLogEntryProxy> entries, Date fromDate, string pattern, bool fill = true)
		{
			Date limit = dateTimeValueFactory.LocalToday(limitAt);

			var q =
				from e in entries
				where Regex.IsMatch(e.Source, pattern)
				// where e.TimeWritten > limit.ToDateTime()
				orderby e.TimeWritten ascending
				group e by new Date(e.TimeWritten) into eg
				where eg.Key >= fromDate
				select new MinMax
				(
					eg.Key,
					eg.Min(e => e.TimeWritten),
					eg.Max(e => e.TimeWritten)
				);

			// Horrible filler. Do some kind of zip or combine or combinatoric variant.
			if (fill)
			{
				var pairs = q.ToDictionary(mm => mm.Date, mm => mm);

				foreach(Date date in Workdays.EnumerateDates(fromDate, dateTimeValueFactory.LocalToday()))
				{
					string key = date.ToString();
					if (pairs.ContainsKey(key))
					{
						continue;
					}
					pairs[key] = new MinMax(date, Option.None<DateTime>(), Option.None<DateTime>());
				}

				q = pairs.Values.OrderBy(p => p.Date);
			}

			// Restrict to min because it sometime confuses itself with daylight savings. Below is a temporary
			// construct.
			return new MinMaxes(q);
		}

	}
}
