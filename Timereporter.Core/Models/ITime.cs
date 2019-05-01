using Optional;
using System;

namespace Timereporter.Core.Models
{
	// Unused
	public interface ITime
	{
		Option<DateTime> Min { get; }
		Option<DateTime> Max { get; }
		string Date { get; }
		string DayOfWeek { get; }
	}
}
