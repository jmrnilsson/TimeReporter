using System.Collections.Generic;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask.Proxies
{
	public class EventLogStub : IEventLogProxy
	{
		public EventLogStub(string log, IEnumerable<IEventLogEntryProxy> entries)
		{
			Log = log;
			Entries = entries;
		}

		public string Log { get; set; }
		public IEnumerable<IEventLogEntryProxy> Entries { get; }
	}
}
