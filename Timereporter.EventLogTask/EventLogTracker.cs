using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Timereporter.Core.Collections;

namespace Timereporter.EventLogTask
{
	public class EventLogTracker
	{
		private readonly IDateTimeValueFactory dateTimeValueFactory;

		public EventLogTracker(IDateTimeValueFactory dateTimeValueFactory)
		{
			this.dateTimeValueFactory = dateTimeValueFactory;
		}

		public string[] FindBy(EventLogQuery query)
		{
			int i;
			string eventLogName = query.LogName;

			EventLog eventLog = ObjectFactory.Instance.EventLog();
			eventLog.Log = eventLogName;
			EventsTimeSource eventTimeSource = new EventsTimeSource(query.Pattern, dateTimeValueFactory.LocalNow);

			i = 0;
			foreach (EventLogEntry log in eventLog.Entries)
			{
				if (i % 1000 == 0) Console.Write(".");
				eventTimeSource.Add(log);
				i++;
			}

			var mondayAgo = WorkdayHelper.GetTwoMondaysAgo(dateTimeValueFactory.LocalNow());
			return eventTimeSource.GetMinMax(mondayAgo).Select(mm => mm.ToString()).ToArray();
		}
	}
}
