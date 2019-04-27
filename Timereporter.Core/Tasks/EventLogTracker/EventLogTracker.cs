using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Timereporter.Core.Tasks.EventLogTracker
{
	public partial class EventLogTracker
	{

		private static void FindBy(EventLogQuery query)
		{
			int i;
			string eventLogName = query.LogName;

			EventLog eventLog = ObjectFactory.Instance.CreateEventLog();
			eventLog.Log = eventLogName;
			EventsTimeSource eventTimeSource = new EventsTimeSource(query.Pattern);

			i = 0;
			foreach (EventLogEntry log in eventLog.Entries)
			{
				if (i % 1000 == 0) Console.Write(".");
				eventTimeSource.Add(log);
				i++;
			}

			Console.WriteLine("");
			i = 0;
			foreach (EventsTimeSource.MinMax item in eventTimeSource.GetMinMax())
			{
				Console.WriteLine(item);
				i++;
			}
		}
	}
}
