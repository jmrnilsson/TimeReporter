using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Api.Collections;
using Timereporter.Core;
using Timereporter.Core.Models;
using Timereporter.Core.Reducers;
using Optional;

namespace Timereporter.Api
{
	public class ApiService : IApiService
	{
		private readonly IWorkdayRepository workdayRepository;
		private readonly IEventRepository eventLog;
		private readonly ICursorRepository cursorRepository;
		private readonly DateTimeZone dateTimeZone;

		public ApiService(IWorkdayRepository workdayRepository, IEventRepository eventLog, ICursorRepository cursorRepository)
		{
			this.workdayRepository = workdayRepository;
			this.eventLog = eventLog;
			this.cursorRepository = cursorRepository;
			this.dateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();

		}

		public Workdays GetWorkdays(int year, int month, DateTimeZone dateTimeZone)
		{
			var fromLocalDate = dateTimeZone.AtStartOfDay(new LocalDate(year, month, 1));
			var exclusiveToLocalDate = dateTimeZone.AtStartOfDay(new LocalDate(year, month, DateTime.DaysInMonth(year, month)).PlusDays(1));
			List<Event> wd = workdayRepository.Find(fromLocalDate.ToInstant(), exclusiveToLocalDate.ToInstant());
			return WorkdayReducer.ToWorkdayList(year, month, wd, dateTimeZone);
		}

		public void CalculateWorkdays()
		{
			var fromSomeLocalDateToInstant = dateTimeZone.AtStartOfDay(new LocalDate(2018, 10, 1)).ToInstant();
			var now = SystemClock.Instance.GetCurrentInstant();
			var events = eventLog.Find(fromSomeLocalDateToInstant, now);
			var workdaySlices = WorkdaySliceReducer.ToWorkdaySlices(events, dateTimeZone);
			workdayRepository.Save(workdaySlices.ToList());
		}

		public Option<Cursor> GetCursor(string cursorType)
		{
			Option<Cursor> cursor = cursorRepository.Find(cursorType);
			return cursor;
		}

		public IEnumerable<Event> GetEvents(long from, long to)
		{
			return eventLog.Find(from, to);
		}

		public void SaveEvents(List<Event> events)
		{
			eventLog.AddRange(events);
		}
	}
}
