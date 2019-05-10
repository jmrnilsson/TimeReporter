using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core;
using Timereporter.Core.Models;
using Timereporter.EventLogTask.Proxies;
using Xunit;
using Moq;
using NodaTime;

namespace Timereporter.EventLogTask.Tests
{
	public class EventLogTrackerTests : IDisposable
	{
		private EventLogTracker tracker;
		private const int startYear = 2011;
		private const int startMonth = 11;
		private const int startDay = 11;
		private readonly DateTime start = new DateTime(startYear, startMonth, startDay);

		public EventLogTrackerTests()
		{
			tracker = new EventLogTracker(MakeEventLogFactory());
		}

		[Fact]
		public void Results_Matches_Specified_Range_Exactly_Keep_Regular_Weekends_With_Fill()
		{
			var q = new EventLogQuery("^ESENT$", "Application", new LocalDate(2011, 11, 11), new LocalDate(2011, 11, 21), fill: true);
			var actual0 = tracker.FindBy(q);
			var actual = actual0.ToSummarizedWorkdays(q.From, q.To, q.Pattern, q.Fill);

			Assert.True(actual.ContainsKey("2011-11-11"));
			Assert.True(actual.ContainsKey("2011-11-18"));
			Assert.True(actual.ContainsKey("2011-11-14"));
			Assert.False(actual.ContainsKey("2011-11-09"));  // Out of bounds, too early
			Assert.False(actual.ContainsKey("2011-11-22"));  // Out of bounds, too late
			Assert.True(actual.ContainsKey("2011-11-12"));  // Saturday
			Assert.True(actual.ContainsKey("2011-11-13"));  // Sunday

			// Examples:
			//Assert.ContainsKey(actual, a => a.ContainsKey("2011-11-14"));  // Midvalue
			//Assert.DoesNotContain(actual, a => a.ContainsKey("2011-11-10"));  // Out of bounds, too early

		}

		[Fact]
		public void Results_Matches_Specified_Range_Exactly_No_Regular_Weekends()
		{
			var q = new EventLogQuery("^ESENT$", "Application", new LocalDate(2011, 11, 10), new LocalDate(2011, 11, 21));
			var actual0 = tracker.FindBy(q);
			var actual = actual0.ToSummarizedWorkdays(q.From, q.To, q.Pattern, q.Fill);

			Assert.True(actual.ContainsKey("2011-11-18"));
			Assert.False(actual.ContainsKey("2011-11-12"));  // Saturday
			Assert.False(actual.ContainsKey("2011-11-13"));  // Sunday
		}

		[Fact]
		public void Results_Matches_Specified_Range_Exactly_No_Official_Holidays()
		{
			tracker = new EventLogTracker(MakeEventLogFactory(2019, 4, 18));
			var q = new EventLogQuery("^ESENT$", "Application", new LocalDate(2019, 4, 18), new LocalDate(2019, 4, 30));
			var actual0  = tracker.FindBy(q);
			var actual = actual0.ToSummarizedWorkdays(q.From, q.To, q.Pattern, q.Fill);

			Assert.True(actual.ContainsKey("2019-04-18"));
			Assert.False(actual.ContainsKey("2019-04-19"));
			Assert.False(actual.ContainsKey("2019-04-20"));
			Assert.False(actual.ContainsKey("2019-04-22"));  // Saturday
			Assert.True(actual.ContainsKey("2019-04-30"));  // Sunday
		}


		private static IEventLogProxy MakeEventLogFactory(int year = startYear, int month = startMonth, int day = startDay)
		{
			IEnumerable<IEventLogEntryProxy> MakeEventLogEntries()
			{
				const int h = 4;
				DateTime start = new DateTime(year, month, day);
				int i = 0;
				while(start.AddHours(i * h) < start.AddDays(20))
				{
					yield return new EventLogEntryStub(start.AddHours(i * h), i % 2 == 0 ? "ESENT" : "SOMETHING ELSE");
					i++;
				}
			}
			IEventLogProxy mock = new EventLogStub("Application", MakeEventLogEntries());
			return mock;
		}

		//private static IDateTimeValueFactory MakeDateTimeValueFactoryMock(int year = startYear, int month = startMonth, int day = startDay)
		//{
		//	Mock<IDateTimeValueFactory> mock = new Mock<IDateTimeValueFactory>();
		//	DateTime start_ = new LocalDateTime(year, month, day);
		//	mock.Setup(m => m.LocalToday(It.IsAny<int>())).Returns(new LocalDate(start_.AddDays(-22)));
		//	return mock.Object;
		//}

		public void Dispose()
		{
			tracker = null;
		}
	}
}
