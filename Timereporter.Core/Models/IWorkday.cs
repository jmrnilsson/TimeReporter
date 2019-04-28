using System;

namespace Timereporter.Core.Models
{
	public interface IWorkday
	{
		string DateText { get; }
		string DayOfWeekText { get; }
		string ArrivalText { get; }
		string BreakText { get; }
		string DepatureText { get; }
		int Week { get; }
		int Total { get; }
		bool Is(DayOfWeek dayOfWeek);
		bool IsWeekend();
	}
}
