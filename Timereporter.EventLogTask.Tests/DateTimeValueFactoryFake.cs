using System;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask.Tests
{
	public class DateTimeValueFactoryFake : IDateTimeValueFactory
	{
		private readonly DateTime start;

		public DateTimeValueFactoryFake(int year, int month, int day)
		{
			this.start = new DateTime(year, month, day);
		}

		public string DayOfWeekText()
		{
			return start.DayOfWeek.ToString();
		}

		public Date LocalToday(int addDays = 0)
		{
			return new Date(start.AddDays(addDays));
		}

		public DateTime UtcNow()
		{
			return start;
		}
	}
}
