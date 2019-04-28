using System;
using Timereporter.Core.Models;

namespace Timereporter.Core
{
	/// <summary>
	/// This interface is merely a front for mocks and tests aside from localization.
	/// </summary>
	public interface IDateTimeValueFactory
	{
		Date LocalToday(int addDays = 0);
		DateTime UtcNow();

		string DayOfWeekText(); 
	}
}
