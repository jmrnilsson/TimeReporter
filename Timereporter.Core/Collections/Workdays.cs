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
			workdays = EnumerableExtensions.WorkdayRange(year, month);
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
