using System.Collections.Generic;
using System.Diagnostics;

namespace Timereporter.EventLogTask.Proxies
{

	public class EventLogProxy : IEventLogProxy
	{
		private readonly EventLog eventLog;

		public EventLogProxy(EventLog eventLog)
		{
			this.eventLog = eventLog;
		}

		public string Log
		{
			get => eventLog.Log;
			set => eventLog.Log = value;
		}

		public IEnumerable<IEventLogEntryProxy> Entries
		{
			get
			{
				foreach(EventLogEntry e in eventLog.Entries)
				{
					yield return new EventLogEntryProxy(e);
				}
			}
		}
	}
}
