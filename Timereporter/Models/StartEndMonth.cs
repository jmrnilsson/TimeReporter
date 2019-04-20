using System;

namespace Timereporter
{
	public struct StartEndMonth
	{
		public StartEndMonth(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}

		public DateTime Start { get; }
		public DateTime End { get; }
	}
}
