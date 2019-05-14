using NodaTime;
using System.Collections.Generic;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.Api.Collections
{
	public interface IWorkdayRepository : IRepository<WorkdayDto, string>
	{
		List<Event> Find(Instant fromDate, Instant exclusiveToDate);

		void Save(List<IWorkdaySlice> slices);
	}
}
