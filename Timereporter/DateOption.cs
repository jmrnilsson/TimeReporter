using NodaTime;

namespace Timereporter
{
	public class DateOption
	{
		public string Name { get; set; }
		public string YearMonth { get; set; }
		public LocalDate Date { get; set; }
	}
}
