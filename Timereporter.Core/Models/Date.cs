using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core.Collections;

namespace Timereporter.Core.Models
{
	public struct Date : IEquatable<Date>, IComparable<Date>
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

		public Date(Date date)
		{
			this.year = date.year;
			this.month = date.month;
			this.day = date.day;
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
			return Is(System.DayOfWeek.Sunday)
				|| Is(System.DayOfWeek.Saturday)
				|| OfficialHolidays.List.Any(Equals);
		}

		public bool Equals(Date other)
		{
			return other.month == month
				&& other.year == year
				&& other.day == day;
		}

		public DateTime ToDateTime()
		{
			return new DateTime(year, month, day);
		}

		public Date With(int addDays = 0)
		{
			return new Date(date.AddDays(addDays));
		}

		public int CompareTo(Date other)
		{
			return ToDateTime().CompareTo(other.ToDateTime());
		}

		public static bool operator > (Date d0, Date d1)
		{
			return d0.CompareTo(d1) > 0;
		}
		public static bool operator < (Date d0, Date d1)
		{
			return d0.CompareTo(d1) < 0;
		}

		public static bool operator >= (Date d0, Date d1)
		{
			return d0.CompareTo(d1) >= 0;
		}
		public static bool operator <= (Date d0, Date d1)
		{
			return d0.CompareTo(d1) <= 0;
		}
	}
}
