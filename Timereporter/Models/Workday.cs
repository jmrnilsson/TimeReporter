namespace Timereporter.Models
{
	public class Workday
    {
        public string DayOfWeek { get; set; }
        public string Date { get; set; }
        public string ArrivalHour { get; set; }
        public string ArrivalMinute { get; set; }
        public string DepatureHour { get; set; }
        public string DepartureMinute { get; set; }
        public string LuncheonBreakHour { get; set; }
        public string Color { get; set; }
    }
}
