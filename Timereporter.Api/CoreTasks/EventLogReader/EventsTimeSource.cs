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
		private readonly string pattern;
		List<LogEntryBox> events;
		DateTime limitAgo;

		/// <summary>
		///  Limit is used for throttling or speeding up things maybe. Dont know.
		/// </summary>
		/// <param name="pattern"></param>
		/// <param name="now"></param>
		/// <param name="limit"></param>
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

		public IReadOnlyList<MinMax> GetMinMax(DateTime fromDate)
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
			return query.SkipWhile(e => e.Min < fromDate).ToList().AsReadOnly();
		}
	}
}
