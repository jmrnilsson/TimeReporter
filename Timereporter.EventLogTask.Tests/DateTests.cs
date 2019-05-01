using System;
using System.Collections.Generic;
using System.Text;
using Timereporter.Core.Models;
using Xunit;

namespace Timereporter.EventLogTask.Tests
{
	public class DateTests
	{
		//[Theory]
		//[InlineData(1, 2, 3, 1, 2, 3, 0)]
		//[InlineData(1902, 2, 3, 1901, 2, 3, 1)]
		//[InlineData(1901, 1, 1, 1901, 12, 1, -1)]
		//[InlineData(1901, 1, 2, 1901, 1, 1, 1)]
		//public void VerifyOperator_EqualsOrGreaterThanOrLessThan(int year, int month, int day, int year0, int month0, int day0, int compareTo)
		//{
		//}

		[Fact]
		public void VerifyOperator_Equals_And_NotEquals()
		{
			var date0 = new Date(2019, 1, 1);
			var date1 = new Date(2019, 1, 1);
			var date2 = new Date(2020, 1, 1);
			Assert.True(date0 == date1);
			Assert.False(date0 != date1);
			Assert.True(date0 != date2);
			Assert.True(date0.Equals(date1));
			Assert.False(date0.Equals(date2));
		}

		[Fact]
		public void VerifyOperator_CompareTo()
		{
			var date0 = new Date(2019, 1, 1);
			var date1 = new Date(2019, 1, 4);
			var date2 = new Date(2019, 1, 1);

			Assert.Equal(0, date0.CompareTo(date2));
			Assert.Equal(-1, date0.CompareTo(date1));
			Assert.Equal(1, date1.CompareTo(date0));
		}

		[Fact]
		public void VerifyOperator_GreaterThanOrEquals()
		{
			var date0 = new Date(2019, 1, 1);
			var date1 = new Date(2019, 1, 4);
			var date2 = new Date(2019, 1, 1);

			Assert.True(date0 >= date2);
			Assert.False(date0 >= date1);
			Assert.True(date1 >= date0);
		}

		[Fact]
		public void VerifyOperator_GreaterThan()
		{
			var date0 = new Date(2019, 1, 1);
			var date1 = new Date(2019, 1, 4);
			var date2 = new Date(2019, 1, 1);

			Assert.False(date0 > date2);
			Assert.False(date0 > date1);
			Assert.True(date1 > date0);
		}

		[Fact]
		public void VerifyOperator_LessThanOrEquals()
		{
			var date0 = new Date(2019, 1, 1);
			var date1 = new Date(2019, 1, 4);
			var date2 = new Date(2019, 1, 1);

			Assert.True(date0 <= date2);
			Assert.True(date0 <= date1);
			Assert.False(date1 <= date0);
		}

		[Fact]
		public void VerifyOperator_LessThan()
		{
			var date0 = new Date(2019, 1, 1);
			var date1 = new Date(2019, 1, 4);
			var date2 = new Date(2019, 1, 1);

			Assert.False(date0 < date2, "eq");
			Assert.True(date0 < date1, "true");
			Assert.False(date1 < date0, "false");
		}
	}
}
