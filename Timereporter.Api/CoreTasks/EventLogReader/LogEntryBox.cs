using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Timereporter.Api.CoreTasks.EventLogReader
{
	public class LogEntryBox
	{
		private readonly EventLogEntry @event;

		public LogEntryBox(EventLogEntry @event)
		{
			this.@event = @event;
		}

		public DateTime TimeWritten => @event.TimeWritten;
		public string Date => @event.TimeWritten.ToString("yyyy-MM-dd");
		public string Source => @event.Source;

		public bool IsWeekday()
		{
			DayOfWeek dayOfWeek = @event.TimeWritten.DayOfWeek;

			if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
			{
				return false;
			}
			return true;
		}
	}
}
