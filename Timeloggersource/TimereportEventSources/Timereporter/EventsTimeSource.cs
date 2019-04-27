 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Timereporter
{
	public class EventsTimeSource : ITimeSource
	{
		const string sourcePattern = "^ESENT$";
		const int limit = 22;
		List<LogEntryBox> events;
		DateTime limitAgo;

		public EventsTimeSource()
		{
			events = new List<LogEntryBox>();
			limitAgo = ObjectFactory.Instance.CreateDateTimeNow().Date.AddDays(-limit);
		}

		public void Add(EventLogEntry @event)
		{
			if (!Regex.IsMatch(@event.Source, sourcePattern)) return;
			var boxedEvent = new LogEntryBox(@event);
			events.Add(@boxedEvent);
		}

		public IReadOnlyList<MinMax> GetMinMax()
		{
			var query =
			(
				from e in events
				where Regex.IsMatch(e.Source, sourcePattern)
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
			var mondayAgo =
			(
				from i in Enumerable.Range(8, 31)
				let maybeMonday = ObjectFactory.Instance.CreateDateTimeNow().Date.AddDays(-i)
				where maybeMonday.DayOfWeek == DayOfWeek.Monday
				select maybeMonday
			).Max();

			return query.SkipWhile(e => e.Min < mondayAgo).ToList().AsReadOnly();
		}

		public class MinMax
		{
			public DateTime Min { get; private set; }
			public DateTime Max { get; private set; }
			
			public MinMax(DateTime min, DateTime max)
			{
				Min = min;
				Max = max; 
			}

			public override string ToString()
			{
				return $"{Min:yyyy-MM-dd}\t{Min:HH:mm}\t{Max:HH:mm}\n";
			}

		}

		public class LogEntryBox
		{
			private readonly EventLogEntry @event;

			public LogEntryBox(EventLogEntry @event)
			{
				this.@event = @event;
			}

			public DateTime TimeWritten => @event.TimeWritten;
			public string Date => @event.TimeWritten.ToString("yyyy-MM-dd");
			public string Source => @event.Source;

			public bool IsWeekday()
			{
				DayOfWeek dayOfWeek = @event.TimeWritten.DayOfWeek;

				if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
				{
					return false;
				}
				return true;
			}
		}
	}
}
