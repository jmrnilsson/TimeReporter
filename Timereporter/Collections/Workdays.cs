using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Models;

namespace Timereporter.Collections
{

	public class Workdays : IReadOnlyList<Workday>
	{
		private readonly Workday[] workdays;

		public int Count => workdays.Length;

 		Workday IReadOnlyList<Workday>.this[int index] => workdays[index];

		private Workdays(int year, int month)
		{
			this.workdays = EnumerateWorkdays(year, month);
		}

		public static IReadOnlyList<Workday> Range(int year, int month)
		{
			return new Workdays(year, month);
		}

		private Workday[] EnumerateWorkdays(int year, int month)
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
					yield return new Workday(date, 0, 0, 0);
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

		IEnumerator<Workday> IEnumerable<Workday>.GetEnumerator()
		{
			return new WorkdayEnum(workdays);
		}
	}
}
