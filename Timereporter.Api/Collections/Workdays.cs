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

		public Core.Models.Workdays Find(Date from, Date to)
		{
			DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

			using (DatabaseContext db = databaseContextFactory())
			{
				var events_ =
					from e in db.Events
					where e.Timestamp >= DoubleExtensions.ToUnixDateTimeMilliseconds(@from.ToDateTime().ToUniversalTime())
					where e.Timestamp < DoubleExtensions.ToUnixDateTimeMilliseconds(to.ToDateTime().ToUniversalTime())
					select ModelFactory.MakeEvent(e.Kind, e.Timestamp);

				// var eventKvp = events_.ToDictionary(e => new Date(DoubleExtensions.ToLocalDateTimestampMilliseconds(e.Timestamp)), e => e);

				return new Core.Models.Workdays
				{
					Workdays_ =
					(
						from e in events_
						group e by new Date(DoubleExtensions.ToLocalDateTimestampMilliseconds(e.Timestamp)) into eg
						let min = eg.FirstOrDefault(e => e.Kind == "esent_min")
						let min_ = min.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(tz).LocalDateTime.TimeOfDay
						let max = eg.FirstOrDefault(e => e.Kind == "esent_max")
						let max_ = max.Timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(tz).LocalDateTime.TimeOfDay
						select new WorkdayDto
						{
							Date = eg.Key.DateText(),
							ArrivalHours = min != null ? (float) min_.Hour + (float) min_.Minute / (float) 60 + (float) min_.Second / (float) 360 : 0,
							BreakHours = 0,
							DepartureHours = max != null ? (float) max_.Hour + (float) max_.Minute / (float) 60 + (float)max_.Second / (float) 360 : 0
						}
					).ToList()
				};
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
