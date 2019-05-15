using System.Collections.Generic;
using NodaTime;
using Optional;
using Timereporter.Core.Models;

namespace Timereporter.Api
{
	public interface IApiService
	{
		void CalculateWorkdays();
		Option<Cursor> GetCursor(string cursorType);
		IEnumerable<Event> GetEvents(long from, long to);
		Workdays GetWorkdays(int year, int month, DateTimeZone dateTimeZone);
		void SaveEvents(List<Event> events);
	}
}