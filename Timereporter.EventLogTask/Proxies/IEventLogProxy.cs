using System.Collections.Generic;

namespace Timereporter.EventLogTask.Proxies
{
	public interface IEventLogProxy
	{
		string Log { get; set; }
		IEnumerable<IEventLogEntryProxy> Entries { get; }
	}
}
