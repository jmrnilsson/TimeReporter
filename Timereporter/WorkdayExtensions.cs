using System;
using System.Collections.Generic;
using Timereporter.Models;

namespace Timereporter
{
	public static class WorkdayExtensions
	{
		public static IEnumerable<int> GetWeekendIndices(this List<Workday> workdays)
		{
			int i = 0;
			while (i < workdays.Count)
			{
				if (workdays[i].Is(DayOfWeek.Sunday))
				{
					yield return i;
				}
				else if (workdays[i].Is(DayOfWeek.Saturday))
				{
					yield return i;
				}
				else if (workdays[i].Is(DayOfWeek.Monday))
				{
					i += 5;
					continue;
				}
				i++;
			}
		}
	}
}
