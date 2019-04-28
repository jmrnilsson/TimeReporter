using System;
using System.Collections.Generic;
using System.Text;

namespace Timereporter.Core.Models
{
	public struct Date
	{
		private readonly int year;
		private readonly int month;
		private readonly int day;
		private readonly DateTime date;

		public Date(int year, int month, int day)
		{
			this.year = year;
			this.month = month;
			this.day = day;
			this.date = new DateTime(year, month, day);
		}

		public Date(DateTime datetime)
		{
			this.year = datetime.Year;
			this.month = datetime.Month;
			this.day = datetime.Day;
			this.date = new DateTime(year, month, day);
		}

		public override string ToString()
		{
			return date.ToString("yyyy-MM-dd");
		}

		public override int GetHashCode()
		{
			return year ^ month ^ day;
		}

		public string DayOfWeek()
		{
			return date.DayOfWeek.ToString();
		}

		public bool Is(DayOfWeek dayOfWeek)
		{
			return date.DayOfWeek == dayOfWeek;
		}

		public bool IsWeekend()
		{
			return Is(System.DayOfWeek.Sunday) || Is(System.DayOfWeek.Saturday);
		}
	}
}
