using System;

namespace Timereporter.Api.Models
{
	public class Workday
	{
		public int WorkdayId { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public string Date { get; set; }
		public DateTime Arrival { get; set; }
		public DateTime Departure { get; set; }
		public int BreakDurationSeconds { get; set; }
	}
}
