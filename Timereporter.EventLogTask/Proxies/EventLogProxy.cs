using System;
using System.Collections.Generic;
using System.Diagnostics;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask.Proxies
{
	public class EventLogProxy : IEventLogProxy, IDisposable
	{
		private readonly Func<EventLog> eventLogFactory;
		private Lazy<EventLog> eventLog;
		
		public EventLogProxy(Func<EventLog> eventLogFactory)
		{
			this.eventLogFactory = eventLogFactory;
			this.eventLog = new Lazy<EventLog>(eventLogFactory);
		}

		public string Log
		{
			get => eventLog.Value.Log;
			set => eventLog.Value.Log = value;
		}

		public IEnumerable<IEventLogEntryProxy> Entries
		{
			get
			{
				foreach(EventLogEntry e in eventLog.Value.Entries)
				{
					yield return new EventLogEntryProxy(e);
				}
			}
		}

		// Try-finally because of the lazy initilization of EventLog and to mitigate any issues
		// arrising from proxy mistakenly are registered with different life-cycle than EventLog.
		public void Dispose()
		{
			if (eventLog.IsValueCreated)
			{
				try
				{
					eventLog.Value.Dispose();
				}
				finally
				{
					eventLog = new Lazy<EventLog>(eventLogFactory());
				}
			}
		}
	}
}
