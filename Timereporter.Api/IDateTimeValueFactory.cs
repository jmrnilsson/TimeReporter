using System;

namespace Timereporter
{
	/// <summary>
	/// This interface is merely a front for mocks and tests aside from localization.
	/// </summary>
	public interface IDateTimeValueFactory
	{
		DateTime LocalNow();
		DateTime UtcNow();

		string DayOfWeekText(); 
	}
}
