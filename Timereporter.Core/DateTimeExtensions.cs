using System;
using System.Collections.Generic;
using System.Text;

namespace Timereporter.Core
{
	public static class DateTimeExtensions
	{
		public static bool IsWeekday(this DateTime dateTime)
		{
			return dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday;
		}
	}
}
