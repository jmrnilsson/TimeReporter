using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	public class MinMaxes
	{
		private readonly IEnumerable<MinMax> minMaxes;

		public MinMaxes(IEnumerable<MinMax> minMaxes)
		{
			this.minMaxes = minMaxes;
		}

		public bool Contains(string date)
		{
			return minMaxes.Any(mm => mm.Date == date);
		}

		/// <summary>
		///  Temporary weekend padded row until perdier fix for this.. O_o
		/// </summary>
		/// <param name="minMaxes"></param>
		/// <returns></returns>
		public string PrettyPrint()
		{
			const int padding = 14;
			var sb = new StringBuilder();
			var count = minMaxes.Count();

			void PadAppend(string text)
			{
				sb.Append(text.PadRight(padding));
			}

			PadAppend("DATE");
			PadAppend("DAY OF WEEK");
			PadAppend("ARRIVAL");
			PadAppend("LEAVE");
			sb.AppendLine();
			sb.AppendLine();

			for (int i = 0; i < count; i++)
			{
				var minMax = minMaxes.ElementAt(i);
				PadAppend(minMax.Date);
				PadAppend(minMax.DayOfWeek);
				PadAppend(minMax.Min.Match(some: m => m.ToString("HH:mm"), none: () => "-"));
				PadAppend(minMax.Max.Match(some: m => m.ToString("HH:mm"), none: () => "-"));
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}
}
