using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timereporter.Api.CoreTasks.EventLogReader
{
	public struct EventLogQuery
	{
		public EventLogQuery(string pattern, string logName, DateTime start)
		{
			Pattern = pattern;
			LogName = logName;
			Start = start;
		}

		public string Pattern { get; }
		public string LogName { get; }
		public DateTime Start { get; }
	}
}
