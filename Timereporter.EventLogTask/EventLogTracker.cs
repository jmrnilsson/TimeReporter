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
	public class EventLogTracker
	{
		private readonly IDateTimeValueFactory dateTimeValueFactory;
		private readonly IEventLogProxy eventLog;

		public EventLogTracker(IDateTimeValueFactory dateTimeValueFactory, IEventLogProxy eventLog)
		{
			this.dateTimeValueFactory = dateTimeValueFactory;
			this.eventLog = eventLog;
		}

		public string[] FindBy(EventLogQuery query)
		{
			List<LogEntryBox> events = new List<LogEntryBox>();
			Date limit = dateTimeValueFactory.LocalToday(-22);

			// ;
			eventLog.Log = query.LogName;


			void Add(IEventLogEntryProxy entry)
			{
				if (!Regex.IsMatch(entry.Source, query.Pattern))
				{
					return;
				}
				var boxedEvent = new LogEntryBox(entry);
				events.Add(@boxedEvent);
			}

			IReadOnlyList<MinMax> GetMinMax(Date fromDate)
			{
				var q =
				(
					from e in events
					where Regex.IsMatch(e.Source, query.Pattern)
					where e.TimeWritten > limit.ToDateTime()
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
				return q.SkipWhile(e => e.Min < fromDate.ToDateTime()).ToList().AsReadOnly();
			}

			int i;
			string eventLogName = query.LogName;

			
			i = 0;
			foreach (IEventLogEntryProxy log in eventLog.Entries)
			{
				if (i % 1000 == 0) Console.Write(".");
				Add(log);
				i++;
			}

			Date mondayAgo = WorkdayHelper.GetTwoMondaysAgo(dateTimeValueFactory.LocalToday());
			return GetMinMax(mondayAgo).Select(mm => mm.ToString()).ToArray();
		}
	}
}
