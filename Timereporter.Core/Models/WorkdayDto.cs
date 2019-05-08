namespace Timereporter.Core.Models
{
	public class WorkdayDto
	{
		public string Date { get; set; }
		public long Changed { get; set; }
		public float? ArrivalHours { get; set; }
		public float? BreakHours { get; set; }
		public float? DepartureHours { get; set; }
	}
}
