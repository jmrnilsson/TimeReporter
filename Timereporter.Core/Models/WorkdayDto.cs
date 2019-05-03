namespace Timereporter.Core.Models
{
	public class WorkdayDto
	{
		public string Date { get; set; }
		public float ArrivalHours { get; set; }
		public string ArrivalConfidence { get; set; }
		public float BreakHours { get; set; }
		public float DepartureHours { get; set; }
		public string DepartureConfidence { get; set; }
	}
}
