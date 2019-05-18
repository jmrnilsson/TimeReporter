using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Api.Collections;
using Timereporter.Core;
using Timereporter.Core.Models;
using Timereporter.Core.Reducers;
using Optional;
using Timereporter.Api.Controllers;
using Optional.Unsafe;

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
			List<IWorkdaySlice> wd = workdayRepository.Find(fromLocalDate.Date, exclusiveToLocalDate.Date);
			return WorkdayReducer.ToWorkdayList(year, month, wd, dateTimeZone);
		}

		//public Workdays GetWorkdays(int year, int month, DateTimeZone dateTimeZone)
		//{
		//	var fromLocalDate = dateTimeZone.AtStartOfDay(new LocalDate(year, month, 1));
		//	var exclusiveToLocalDate = dateTimeZone.AtStartOfDay(new LocalDate(year, month, DateTime.DaysInMonth(year, month)).PlusDays(1));
		//	List<Event> wd = workdayRepository.Find(fromLocalDate.ToInstant(), exclusiveToLocalDate.ToInstant());
		//	return WorkdayReducer.ToWorkdayList(year, month, wd, dateTimeZone);
		//}

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

		public TransactionResult TrySaveWorkdaySlice(DateText dateText, WorkdaySliceProperty prop, decimal hourOfDay, string hashCode)
		{
			Option<long> hourOfDay_ =  ((long)(hourOfDay * 1000m * 60m * 60m)).Some();

			var optionalWorkday = workdayRepository.Find(dateText.ToInt32());

			WorkdaySlice slice = optionalWorkday.ValueOrFailure();

			if (slice.HashCode != hashCode)
			{
				return TransactionResult.Conflict;
			}

			if (prop == WorkdaySliceProperty.Arrival)
			{
				slice.Arrival = hourOfDay_;
			}
			else if (prop == WorkdaySliceProperty.Break)
			{
				slice.Break = hourOfDay_;
			}
			else if (prop == WorkdaySliceProperty.Departure)
			{
				slice.Departure = hourOfDay_;
			}

			if (slice.HashCode == slice.CalculateHashCode())
			{
				return TransactionResult.NotModified;
			}

			workdayRepository.Save(slice);
			return TransactionResult.Ok;
		}
	}
}
