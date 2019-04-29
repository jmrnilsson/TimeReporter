using System;

namespace Timereporter.EventLogTask.Proxies
{
	public class EventLogEntryStub : IEventLogEntryProxy
	{
		public EventLogEntryStub(DateTime timeWritten, string source)
		{
			TimeWritten = timeWritten;
			Source = source;
		}

		public DateTime TimeWritten { get; }
		public string Source { get; }
	}
}
