using NodaTime;
using NodaTime.Extensions;
using System;
using Timereporter.Core.Models;

namespace Timereporter.Core
{
	public static class DoubleExtensions
	{
		public static Instant ToInstantFromUnixTimestampMilliseconds(this long unixTimeStamp)
		{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return dt.AddMilliseconds(unixTimeStamp).ToInstant();
		}

		public static long ToUnixDateTimeMilliseconds(this DateTime utc)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long) (utc - epoch).TotalMilliseconds;
		}

		public static Date ToLocalDateTimestampMilliseconds(this long unixTimeStamp)
		{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			LocalDateTime local = dt.AddMilliseconds(unixTimeStamp).ToLocalDateTime();
			return new Date(local);
		}
	}
}
