using Optional;

namespace Timereporter.Core.Models
{
	public class WorkdaySlice : IWorkdaySlice
	{
		public string Date { get; set; }
		public string Kind { get; set; }
		public Option<long> Arrival { get; set; }
		public Option<long> Break { get; set; }
		public Option<long> Departure { get; set; }
		public string HashCode { get; set; }
	}
}

