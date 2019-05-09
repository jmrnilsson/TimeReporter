using Optional;

namespace Timereporter.Core.Models
{
	public class WorkdayDto
	{
		public string Date { get; set; }
		public long Changed { get; set; }
		public Option<float> ArrivalHours { get; set; }
		public Option<float> BreakHours { get; set; }
		public Option<float> DepartureHours { get; set; }
		public TimeConfidence ArrivalConfidence { get; set; }
		public TimeConfidence DepartureConfidence { get; set; }
	}
}
