﻿using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;

namespace Timereporter.Core
{
	public class WorkdaySummarizer
	{

		public Models.Workdays Summarize(int year, int month, List<Event> events, DateTimeZone tz)
		{
			IEnumerable<WorkdayDetailsDto> Enumerate_()
			{
				var everyDay = WorkdayHelper.Range(year, month);
				var workdayKvp_ = EnumerableExtensions.ToWorkdays(events, tz);
				var workdayKvp = workdayKvp_.ToWorkdayDetails();
				var workKvp = workdayKvp.ToDictionary(wd => wd.Date);
				var weeks = EnumerableExtensions.GetEuropeanWeeks(year);

				for (int i = 0; i < everyDay.Count; i++)
				{
					IWorkday wd = everyDay[i];
					DateTime localDateTime = DateTime.ParseExact(wd.DateText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
					LocalDate localDate = new LocalDate(localDateTime.Year, localDateTime.Month, localDateTime.Day);
					if (workKvp.ContainsKey(wd.DateText) && !wd.IsWeekend())
					{
						yield return new WorkdayDetailsDto
						{
							Date = wd.DateText,
							DayOfWeek = wd.DayOfWeekText,
							ArrivalHours = workKvp[wd.DateText].ArrivalHours,
							BreakHours = workKvp[wd.DateText].BreakHours,
							DepartureHours = workKvp[wd.DateText].DepartureHours,
							Total = workKvp[wd.DateText].Total,
							ArrivalConfidence = workKvp[wd.DateText].ArrivalConfidence,
							DepartureConfidence = workKvp[wd.DateText].DepartureConfidence,
							IsWeekend = wd.IsWeekend(),
							WeekNumber = weeks[localDate]
						};
					}
					else
					{
						yield return new WorkdayDetailsDto
						{
							Date = wd.DateText,
							DayOfWeek = wd.DayOfWeekText,
							ArrivalHours = "",
							BreakHours = "",
							DepartureHours = "",
							Total = "",
							IsWeekend = wd.IsWeekend(),
							WeekNumber = weeks[localDate]
						};
					}
				}
			}

			return new Models.Workdays { List = Enumerate_().ToList() };

		}
	}
}