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

		public List<IEventLogEntryProxy> FindBy(EventLogQuery query)
		{
			eventLog.Log = query.LogName;
			return eventLog.Entries.ToList(ReportProgress, e => Regex.IsMatch(e.Source, query.Pattern));
		}

		private void ReportProgress(int i, IEventLogEntryProxy e)
		{
			if (OnProgressChanged != null && i % reportProgressBy == 0)
			{
				OnProgressChanged();
			}
		}

	}
}
