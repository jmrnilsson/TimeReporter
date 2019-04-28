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

		public override string ToString()
		{
			var sb = new StringBuilder();
			void PadAppend(string text)
			{
				sb.Append(text.PadLeft(12));
			}

			PadAppend(Min.ToString("yyyy-MM-dd"));
			PadAppend(Min.DayOfWeek.ToString());
			PadAppend(Min.ToString("HH:mm"));
			PadAppend(Max.ToString("HH:mm"));

			return sb.ToString();
		}

	}
}
