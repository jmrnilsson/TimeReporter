using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Timereporter.EventLogTask.Proxies;

namespace Timereporter.EventLogTask
{
	public class LogEntryBox
	{
		private readonly IEventLogEntryProxy entry;

		public LogEntryBox(IEventLogEntryProxy entry)
		{
			this.entry = entry;
		}

		public DateTime TimeWritten => entry.TimeWritten;
		public string Date => entry.TimeWritten.ToString("yyyy-MM-dd");
		public string Source => entry.Source;


	}
}
