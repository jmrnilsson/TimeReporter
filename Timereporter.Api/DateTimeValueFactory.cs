using System;

namespace Timereporter
{
	public class DateTimeValueFactory : IDateTimeValueFactory
	{
		string IDateTimeValueFactory.DayOfWeekText()
		{
			var culture = new System.Globalization.CultureInfo("en-US");
			return culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
		}

		DateTime IDateTimeValueFactory.LocalNow()
		{
			return DateTime.Now;
		}

		DateTime IDateTimeValueFactory.UtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}
