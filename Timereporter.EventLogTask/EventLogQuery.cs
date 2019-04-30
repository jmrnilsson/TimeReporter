using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	public struct EventLogQuery
	{
		public EventLogQuery(string pattern, string logName, Date fromDate, Date toDate, bool fill = false)
		{
			Pattern = pattern;
			LogName = logName;
			From = fromDate;
			To = toDate;
			Fill = fill;
		}

		public string Pattern { get; }
		public string LogName { get; }
		public Date From { get; }
		public Date To { get; }
		public bool Fill { get; }
	}
}
