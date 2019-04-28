using System;

namespace Timereporter.EventLogTask.Proxies
{
	public interface IEventLogEntryProxy
	{
		DateTime TimeWritten { get; }
		string Source { get; }
	}
}
