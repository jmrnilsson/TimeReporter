namespace Timereporter.Api.Collections.Queries
{
	public struct YearMonth
	{
		public YearMonth(int year, int month)
		{
			Year = year;
			Month = month;
		}

		public int Year { get; }
		public int Month { get; }
	}
}
