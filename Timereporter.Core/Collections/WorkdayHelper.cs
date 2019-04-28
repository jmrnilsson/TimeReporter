using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core.Models;

namespace Timereporter.Core.Collections
{
	public static class WorkdayHelper
	{
		public static Date GetTwoMondaysAgo(Date fromDate)
		{
			// Pretty bad way of doing this. 
			var q =
				from i in Enumerable.Range(8, 31)
				let maybeMonday = fromDate.With(-i)
				// where maybeMonday.Is(DayOfWeek.Monday)
				select maybeMonday;

			return q.Max();
		}

		public static IReadOnlyList<IWorkday> Range(int year, int month)
		{
			return new Workdays(year, month);
		}
	}
}
