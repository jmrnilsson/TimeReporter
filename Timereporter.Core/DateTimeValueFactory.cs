using System;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.Core
{
	public class DateTimeValueFactory : IDateTimeValueFactory
	{
		string IDateTimeValueFactory.DayOfWeekText()
		{
			var culture = new System.Globalization.CultureInfo("en-US");
			return culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);
		}

		Date IDateTimeValueFactory.LocalToday(int addDays = 0)
		{
			return new Date(DateTime.Now.AddDays(addDays));
		}

		DateTime IDateTimeValueFactory.UtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}
