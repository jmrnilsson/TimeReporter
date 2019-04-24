using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Models;

namespace Timereporter
{

	public class Workdays : IEnumerable
	{
		private readonly Workday[] workdays;

		public Workdays(Workday[] workdays)
		{
			this.workdays = workdays;
		}

		public Workdays(int year, int month)
		{
			this.workdays = EnumerateWorkdays();
		}

		//public GridMetadata GetData()
		//{
		//	return new GridMetadata
		//	{
		//		Workdays = workdays,
		//		// WeekendIndices = new HashSet<int>(workdays.GetWeekendIndices()),
		//	};
		//}


		private Workday[] EnumerateWorkdays()
		{
			DateTime start, end;
			{
				var now = DateTime.Now;
				var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
				start = new DateTime(now.Year, now.Month, 1);
				end = new DateTime(now.Year, now.Month, daysInMonth);
			}

			IEnumerable<Workday> EnumerateWorkdays_()
			{
				for (int i = 0; start.AddDays(i) < end.AddDays(1); i++)
				{
					var date = start.AddDays(i);
					yield return new Workday(date, 0, 0, 0);
				}
			}

			return EnumerateWorkdays_().ToArray();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator) GetEnumerator();
		}

		public WorkdayEnum GetEnumerator()
		{
			return new WorkdayEnum(workdays);
		}
	}
}
