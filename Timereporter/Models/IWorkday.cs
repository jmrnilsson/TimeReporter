namespace Timereporter.Models
{
	public interface IWorkday
	{
		string DayOfWeekText { get; }
		string DateText { get; }
		string ArrivalText { get; }
		string BreakText { get; }
		string DepatureText { get; }
		int Week { get; }
		int Total { get; }
	}
}
