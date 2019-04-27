using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Timereporter
{
	public partial class EventsTimeSource : ITimeSource
	{
		const int limit = 22;
		List<LogEntryBox> events;
		DateTime limitAgo;

		public EventsTimeSource(string pattern)
		{
			events = new List<LogEntryBox>();
			limitAgo = ObjectFactory.Instance.CreateDateTimeNow().Date.AddDays(-limit);
		}

		public void Add(EventLogEntry @event)
		{
			if (!Regex.IsMatch(@event.Source, pattern)) return;
			var boxedEvent = new LogEntryBox(@event);
			events.Add(@boxedEvent);
		}

		public IReadOnlyList<MinMax> GetMinMax()
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
			return query.SkipWhile(e => e.Min < mondayAgo).ToList().AsReadOnly();
		}
	}
}
