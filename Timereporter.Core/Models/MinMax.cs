using Optional;
using System;

namespace Timereporter.Core.Models
{

	public class MinMax
	{
		private readonly Date date;
		private readonly Option<DateTime> min;
		private readonly Option<DateTime> max;

		public Option<DateTime> Min => date.IsWeekend() ? Option.None<DateTime>() : min;
		public Option<DateTime> Max => date.IsWeekend() ? Option.None<DateTime>() : max;

		public string Date => date.ToString();
		public string DayOfWeek => date.DayOfWeek();

		public MinMax(Date date, DateTime min, DateTime max)
		{
			this.date = date;
			this.min = min.Some();
			this.max = max.Some();
		}

		public MinMax(Date date, Option<DateTime> min, Option<DateTime> max)
		{
			this.date = date;
			this.min = min;
			this.max = max;
		}
	}
}
