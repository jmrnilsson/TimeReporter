using System.Collections.Generic;
using NodaTime;
using Timereporter.Core.Models;

namespace Timereporter.Api.Collections
{
	public interface IEventRepository
	{
		void AddRange(IEnumerable<Event> events);
		List<Event> Find(long fromDate, long toDate);
		List<Event> Find(Instant fromInstant, Instant toInstant);
	}
}
