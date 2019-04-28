using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	public struct EventLogQuery
	{
		public EventLogQuery(string pattern, string logName, Date fromDate)
		{
			Pattern = pattern;
			LogName = logName;
			FromDate = fromDate;
		}

		public string Pattern { get; }
		public string LogName { get; }
		public Date FromDate { get; }
	}
}
