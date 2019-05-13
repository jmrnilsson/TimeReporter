namespace Timereporter.Core.Models
{
	public class WorkdayDetailsDto
	{
		public string Date { get; set; }
		public string DayOfWeek { get; set; }
		public string ArrivalHours { get; set; }
		public string BreakHours { get; set; }
		public string DepartureHours { get; set; }
		public string ArrivalConfidence { get; set; }
		public string DepartureConfidence { get; set; }
		public string Total { get; set; }
		public bool IsWeekend { get; set; }
		public int WeekNumber { get; set; }
	}

	public static class WorkdayDetailsDtoExtensions
	{
		public static WorkdayDetailsDto Empty
		(
			string sateText,
			string dayOfWeekText,
			bool isWeekend,
			int weekNumber
		)
		{
			return new WorkdayDetailsDto
			{
				Date = sateText,
				DayOfWeek = dayOfWeekText,
				ArrivalHours = "",
				BreakHours = "",
				DepartureHours = "",
				Total = "",
				IsWeekend = isWeekend,
				WeekNumber = weekNumber
			};
		}

	}
}
