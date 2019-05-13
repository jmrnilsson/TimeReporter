using NodaTime;

namespace Timereporter.EventLogTask
{
	public struct EventLogQuery
	{
		public EventLogQuery(string pattern, string logName, LocalDate fromDate, LocalDate toDate, bool fill = false)
		{
			Pattern = pattern;
			LogName = logName;
			From = fromDate;
			To = toDate;
			Fill = fill;
		}

		public string Pattern { get; }
		public string LogName { get; }
		public LocalDate From { get; }
		public LocalDate To { get; }
		public bool Fill { get; }
	}
}
