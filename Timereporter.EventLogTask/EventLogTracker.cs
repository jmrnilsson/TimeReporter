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

		public IEnumerable<List<Event>> Chunkmap(Dictionary<string, Time> times)
		{
			List<Event> events = new List<Event>();
			
			foreach(var t in times)
			{
				if (events.Count > 998)
				{
					yield return events;
					events.Clear();
				}

				// TODO: Replace with Some(Action<>) me thinks
				t.Value.Min.Match(some: min => events.Add(ModelFactory.MakeEvent("esent_min", min)), none: () => { });
				t.Value.Max.Match(some: max => events.Add(ModelFactory.MakeEvent("esent_max", max)), none: () => { });
			}
			yield return events;
		}

		public Dictionary<string, Time> FindBy(EventLogQuery query)
		{
			eventLog.Log = query.LogName;

			var entries = eventLog.Entries.ToListAndTap(ReportProgress, e => Regex.IsMatch(e.Source, query.Pattern));
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

		private IEnumerable<Time> Summarize(List<IEventLogEntryProxy> entries, Date from, Date to, string pattern)
		{
			Instant now = SystemClock.Instance.GetCurrentInstant();
			DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

			return
				from e in entries
				where Regex.IsMatch(e.Source, pattern)
				orderby e.TimeWritten ascending
				group e by new Date(e.TimeWritten) into eg
				where !eg.Key.IsWeekend()
				where eg.Key >= @from
				where eg.Key <= to
				select new Time
				(
					eg.Key,
					eg.Min(e => e.TimeWritten),
					eg.Max(e => e.TimeWritten),
					tz
				);
		}

		IEnumerable<Time> Fill(IEnumerable<Time> minMaxes, Date from, Date to)
		{
			var kvp = minMaxes.ToDictionary(mm => mm.Date, mm => mm);

			foreach (Date date in Core.Collections.Workdays.EnumerateDates(from, to))
			{
				yield return kvp.GetValueOrDefault(date.DateText(), new Time(date, Option.None<Instant>(), Option.None<Instant>()));
			}
		}
	}
}
