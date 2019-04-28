using System;
using System.Diagnostics;

namespace Timereporter.EventLogTask.Proxies
{

	public class EventLogEntryProxy : IEventLogEntryProxy
	{
		public EventLogEntryProxy(EventLogEntry entry)
		{
			TimeWritten = entry.TimeWritten;
			Source = entry.Source;
		}

		public DateTime TimeWritten { get; }
		public string Source { get; }
	}
}
