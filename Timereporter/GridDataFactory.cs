using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Models;

namespace Timereporter
{
	public class GridDataFactory
	{
		public GridMetadata GetData()
		{
			var startEndOfMonth = StartEndOfMonth();
			var workdays = EnumerateWorkdays(startEndOfMonth).ToList();

			return new GridMetadata
			{
				Workdays = workdays,
				WeekendIndices = new HashSet<int>(workdays.GetWeekendIndices()),
			};
		}

		private StartEndMonth StartEndOfMonth()
		{
			var now = DateTime.Now;
			var startOfMonth = new DateTime(now.Year, now.Month, 1);
			var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
			var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth);
			return new StartEndMonth(startOfMonth, endOfMonth);
		}

		private IEnumerable<Workday> EnumerateWorkdays(StartEndMonth startEndMonth)
		{
			for (int i = 0; startEndMonth.Start.AddDays(i) < startEndMonth.End.AddDays(1); i++)
			{
				var date = startEndMonth.Start.AddDays(i);
				yield return new Workday(date, 0, 0, 0);
			}
		}
	}
}
