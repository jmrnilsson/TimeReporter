using System;
using System.Collections.Generic;
using Timereporter.Models;

namespace Timereporter
{
	public static class WorkdayExtensions
	{
		public static bool IsWeekend(this Workday workday)
		{
			return workday.Is(DayOfWeek.Sunday) || workday.Is(DayOfWeek.Saturday);
		}

		static HashSet<int> GetWeekendIndices(this Workday[] workdays)
		{
			IEnumerable<int> GetWeekendIndices_()
			{
				int i = 0;
				while (i < workdays.Length)
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

			return new HashSet<int>(GetWeekendIndices_());
		}
	}
}
