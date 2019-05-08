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
}
