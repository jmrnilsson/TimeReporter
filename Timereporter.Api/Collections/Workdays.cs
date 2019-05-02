using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Timereporter.Api.Models;
using Timereporter.Core;
using Timereporter.Core.Models;
using Event = Timereporter.Core.Models.Event;

namespace Timereporter.Api.Collections
{
	public class Workdays
	{
		private readonly DatabaseContextFactoryDelegate databaseContextFactory;

		public Workdays(DatabaseContextFactoryDelegate databaseContextFactory)
		{
			this.databaseContextFactory = databaseContextFactory;
		}

		public List<Event> Find(Instant fromDate, Instant exclusiveToDate)
		{
			// DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

			using (DatabaseContext db = databaseContextFactory())
			{
				IQueryable<Event> events =
					from e in db.Events
					where e.Timestamp >= fromDate.ToUnixTimeMilliseconds()
					where e.Timestamp < exclusiveToDate.ToUnixTimeMilliseconds()
					select new Event(e.Kind, e.Timestamp);

				return events.ToList();
			}
		}

		private double ToUtc(long unixTimestampMilis)
		{
			var dateTime = new DateTime(2015, 05, 24, 10, 2, 0, DateTimeKind.Local);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var unixDateTime = (dateTime.ToUniversalTime() - epoch).TotalSeconds;

			return unixDateTime;
		}
	}
}
