using System.Collections.Generic;
using NodaTime;
using Optional;
using Timereporter.Api.Controllers;
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
		TransactionResult TrySaveWorkdaySlice(DateText dateText, WorkdaySliceProperty property, decimal hourOfDay, string hashCode);
	}
}
