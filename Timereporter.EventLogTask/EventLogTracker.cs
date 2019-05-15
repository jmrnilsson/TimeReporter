using NodaTime;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Timereporter.Api.Collections;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	public delegate void ProgressChanged();

	public class WindowsEventLogReader : IWindowsEventLogReader
	{
		private const int reportProgressBy = 1000;
		private readonly IEventLogProxy eventLog;
		public event ProgressChanged OnProgressChanged;

		public WindowsEventLogReader(IEventLogProxy eventLog)
		{
			this.eventLog = eventLog;
		}

		public List<IEventLogEntryProxy> ReadAll(EventLogQuery query)
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
