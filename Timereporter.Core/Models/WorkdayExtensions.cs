using System;
using System.Collections.Generic;
using Timereporter.Core.Models;

namespace Timereporter.Core.Models
{
	public static class WorkdayExtensions
	{
		public static bool IsWeekend(this Workday workday)
		{
			return workday.Is(DayOfWeek.Sunday) || workday.Is(DayOfWeek.Saturday);
		}
	}
}
