using System;
using System.Text;

namespace Timereporter
{
	public partial class EventsTimeSource
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
				int padLength = 0;
				var sb = new StringBuilder();
				sb.Append(Min.ToString("yyyy-MM-dd").PadLeft(padLength += 30));
				sb.Append(Min.DayOfWeek.ToString().PadLeft(padLength += 30));
				sb.Append(Min.ToString("HH:mm").PadLeft(padLength += 30));
				sb.Append(Max.ToString("HH:mm").PadLeft(padLength += 30));
				return sb.ToString();
			}

		}
	}
}
