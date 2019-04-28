using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.Core.Collections
{
	public static class OfficialHolidays
	{
		private static readonly Lazy<IReadOnlyList<Date>> lazy = new Lazy<IReadOnlyList<Date>>(() => EnumerateOfficialHolidays());

		private static IReadOnlyList<Date> EnumerateOfficialHolidays()
		{
			IEnumerable<Date> GetOfficialHolidays_()
			{
				yield return new Date(2019, 01, 01);
				yield return new Date(2019, 01, 06);
				yield return new Date(2019, 04, 19);
				yield return new Date(2019, 04, 21);
				yield return new Date(2019, 04, 22);
				yield return new Date(2019, 05, 01);
				yield return new Date(2019, 05, 30);
				yield return new Date(2019, 06, 06);
				yield return new Date(2019, 06, 09);
				yield return new Date(2019, 06, 22);
				yield return new Date(2019, 11, 02);
				yield return new Date(2019, 12, 25);
				yield return new Date(2019, 12, 26);
			}
			return GetOfficialHolidays_().ToArray();
		}

		public static IReadOnlyList<Date> List
		{
			get { return lazy.Value; }			
		}
	}
}
