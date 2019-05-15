using System.Collections.Generic;
using NodaTime;
using Optional;
using Timereporter.Core.Models;

namespace Timereporter.Api.Collections
{
	public interface IWorkdayRepository
	{
		List<Event> Find(Instant fromDate, Instant exclusiveToDate);
		Option<WorkdayDto> Find(string date);
		void Save(List<IWorkdaySlice> slices);
	}
}