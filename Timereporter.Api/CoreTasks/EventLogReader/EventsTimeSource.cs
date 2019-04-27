using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Timereporter.Api.CoreTasks.EventLogReader
{
	public class EventsTimeSource : ITimeSource
	{
		const int limit = 22;
		private readonly string pattern;
		List<LogEntryBox> events;
		DateTime limitAgo;

		public EventsTimeSource(string pattern, Func<DateTime> now, int limit = 22)
		{
			events = new List<LogEntryBox>();
			limitAgo = now().Date.AddDays(-limit);
			this.pattern = pattern;
			// limitAgo = fromDate;
		}

		public void Add(EventLogEntry @event)
		{
			if (!Regex.IsMatch(@event.Source, pattern)) return;
			var boxedEvent = new LogEntryBox(@event);
			events.Add(@boxedEvent);
		}

		public IReadOnlyList<MinMax> GetMinMax(DateTime toDate)
		{
			var query =
			(
				from e in events
				where Regex.IsMatch(e.Source, pattern)
				where e.TimeWritten > limitAgo
				where e.IsWeekday()
				orderby e.TimeWritten ascending
				group e by e.Date into eg
				select new MinMax
				(
					eg.Min(e => e.TimeWritten),
					eg.Max(e => e.TimeWritten)
				)
			).ToList();

			// Max, because seems confused about DLS
			return query.SkipWhile(e => e.Min < toDate).ToList().AsReadOnly();
		}
	}
}
