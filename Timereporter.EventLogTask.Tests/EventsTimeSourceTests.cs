using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Timereporter.Core;
using Timereporter.Core.Models;
using Timereporter.EventLogTask.Proxies;
using Xunit;

namespace Timereporter.EventLogTask.Tests
{
	// Microsoft.NET.Test.Sdk
	public class EventLogTrackerTests
	{
		EventLogTracker tracker = new EventLogTracker(MakeDateTimeValueFactoryMock(), MakeEventLogFactory());

		[Fact]
		public void Results_Matches_Specified_Range_Exactly()
		{
			string[] actual = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", new Date(2011, 11, 10)));

			Assert.Contains("2011-11-11", actual[0]);
			Assert.Contains("2011-11-21", actual.Last());

			bool foundMidvalue = false;
			foreach(var a in actual)
			{
				Assert.DoesNotContain("2011-11-10", a);
				Assert.DoesNotContain("2011-11-22", a);

				if (a.Contains("2011-11-12"))
				{
					foundMidvalue = true;
				}
			}
			Assert.True(foundMidvalue);
		}

		private static IEventLogProxy MakeEventLogFactory()
		{
			IEnumerable<IEventLogEntryProxy> MakeEventLogEntries()
			{
				const int h = 4;
				DateTime start = new DateTime(2011, 11, 11);
				DateTime end = new DateTime(2011, 11, 21);
				int i = 0;
				while(start.AddHours(i*h) < end)
				{
					yield return new EventLogEntryStub(start.AddDays(i), i % 2 == 0 ? "ESENT" : "SOMETHING ELSE");
					i++;
				}
			}
			IEventLogProxy mock = new EventLogStub("Application", MakeEventLogEntries());
			return mock;
		}

		private static IDateTimeValueFactory MakeDateTimeValueFactoryMock()
		{
			DateTime start = new DateTime(2011, 11, 11);
			Mock<IDateTimeValueFactory> mock = new Mock<IDateTimeValueFactory>();
			mock.Setup(m => m.LocalToday(It.IsAny<int>())).Returns(new Date(start.AddDays(-22)));
			return mock.Object;
		}
	}
}
