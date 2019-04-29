using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core;
using Timereporter.Core.Models;
using Timereporter.EventLogTask.Proxies;
using Xunit;
using Moq;
using FluentAssertions;

namespace Timereporter.EventLogTask.Tests
{
	public class EventLogTrackerTests
	{
		private EventLogTracker tracker = new EventLogTracker(MakeDateTimeValueFactoryMock(), MakeEventLogFactory());
		private const int startYear = 2011;
		private const int startMonth = 11;
		private const int startDay = 11;
		private readonly DateTime start = new DateTime(startYear, startMonth, startDay);


		[Fact]
		public void Results_Matches_Specified_Range_Exactly_No_Regular_Weekends()
		{
			string[] actual = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", new Date(2011, 11, 10)));

			Assert.Contains("2011-11-11", actual[0]);
			Assert.Contains("2011-11-18", actual.Last());
			Assert.Contains(actual, a => a.Contains("2011-11-14"));  // Midvalue
			Assert.DoesNotContain(actual, a => a.Contains("2011-11-10"));  // Out of bounds, too early
			Assert.DoesNotContain(actual, a => a.Contains("2011-11-22"));  // Out of bounds, too late
			Assert.DoesNotContain(actual, a => a.Contains("2011-11-12"));  // Saturday
			Assert.DoesNotContain(actual, a => a.Contains("2011-11-13"));  // Sunday
		}

		private static IEventLogProxy MakeEventLogFactory()
		{
			IEnumerable<IEventLogEntryProxy> MakeEventLogEntries()
			{
				const int h = 4;
				DateTime start = new DateTime(startYear, startMonth, startDay);
				int i = 0;
				while(start.AddHours(i*h) < start.AddDays(10))
				{
					yield return new EventLogEntryStub(start.AddHours(i * h), i % 2 == 0 ? "ESENT" : "SOMETHING ELSE");
					i++;
				}
			}
			IEventLogProxy mock = new EventLogStub("Application", MakeEventLogEntries());
			return mock;
		}

		private static IDateTimeValueFactory MakeDateTimeValueFactoryMock()
		{
			Mock<IDateTimeValueFactory> mock = new Mock<IDateTimeValueFactory>();
			DateTime start_ = new DateTime(startYear, startMonth, startDay);
			mock.Setup(m => m.LocalToday(It.IsAny<int>())).Returns(new Date(start_.AddDays(-22)));
			return mock.Object;
		}
	}
}
