using System;

namespace Timereporter.Core.Tasks.EventLogTracker
{
	public partial class EventLogTracker
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
}
