using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;

namespace Timereporter.Core.Collections
{
	public class Workdays : IReadOnlyList<IWorkday>
	{
		private readonly IWorkday[] workdays;

		public int Count => workdays.Length;

		IWorkday IReadOnlyList<IWorkday>.this[int index] => workdays[index];

		public Workdays(int year, int month)
		{
			workdays = EnumerateWorkdays(year, month);
		}
			   
		private IWorkday[] EnumerateWorkdays(int year, int month)
		{
			DateTime start, end;
			{
				var daysInMonth = DateTime.DaysInMonth(year, month);
				start = new DateTime(year, month, 1);
				end = new DateTime(year, month, daysInMonth);
			}

			IEnumerable<Workday> EnumerateWorkdays_()
			{
				for (int i = 0; start.AddDays(i) < end.AddDays(1); i++)
				{
					var date = start.AddDays(i);
					yield return new Workday(new Date(date), 0, 0, 0);
				}
			}

			return EnumerateWorkdays_().ToArray();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public WorkdayEnum GetEnumerator()
		{
			return new WorkdayEnum(workdays);
		}

		IEnumerator<IWorkday> IEnumerable<IWorkday>.GetEnumerator()
		{
			return new WorkdayEnum(workdays);
		}
	}
}
