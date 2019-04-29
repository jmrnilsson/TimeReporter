using System;
using System.Text;

namespace Timereporter.EventLogTask
{
	public class MinMax
	{
		public DateTime Min { get; private set; }
		public DateTime Max { get; private set; }

		public MinMax(DateTime min, DateTime max)
		{
			Min = min;
			Max = max;
		}
	}
}
