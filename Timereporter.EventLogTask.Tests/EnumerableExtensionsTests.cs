using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using Timereporter.Core;
using Xunit;

namespace Timereporter.Tests
{
	public class EnumerableExtensionsTests
	{
		[Theory]
		[InlineData("8,0")]
		[InlineData("8.0")]
		[InlineData("8,0 ")]
		public void Verify_8_0_20110102(string expression)
		{
			var tdz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			var timestamp = expression.FromHourDecimalExpressionToUnixTimestampMilliseconds(new LocalDate(2011, 1, 2), tdz);

			var less = new LocalDateTime(2011, 1, 2, 7, 59);
			var more = new LocalDateTime(2011, 1, 2, 8, 1);

			var someTime = timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(tdz).LocalDateTime;

			Assert.InRange(someTime, less, more);
		}

		[Fact]
		public void Verify_0_1_20110218()
		{
			var tdz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			var timestamp = "0.1".FromHourDecimalExpressionToUnixTimestampMilliseconds(new LocalDate(2011, 2, 18), tdz);

			var less = new LocalDateTime(2011, 2, 17, 23, 59);
			var more = new LocalDateTime(2011, 2, 18, 0, 15);

			var someTime = timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(tdz).LocalDateTime;

			Assert.InRange(someTime, less, more);
		}

		[Fact]
		public void Verify_23_9_20110101()
		{
			var tdz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			var timestamp = "23.9".FromHourDecimalExpressionToUnixTimestampMilliseconds(new LocalDate(2011, 1, 1), tdz);

			var less = new LocalDateTime(2011, 1, 1, 23, 45);
			var more = new LocalDateTime(2011, 1, 2, 1, 1);

			var someTime = timestamp.ToInstantFromUnixTimestampMilliseconds().InZone(tdz).LocalDateTime;

			Assert.InRange(someTime, less, more);
		}
	}
}
