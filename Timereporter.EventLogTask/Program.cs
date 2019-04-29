using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	class Program
	{
		static void Main(string[] args)
		{
			var tracker = ObjectFactory.Instance.EventLogTracker();
			tracker.OnProgressChanged += Tracker_OnProgressChanged;
			var dateTimeValueFactory = ObjectFactory.Instance.DateTimeValueFactory();
			Date mondayAgo = WorkdayHelper.GetTwoMondaysAgo(dateTimeValueFactory.LocalToday());
			IReadOnlyList<MinMax> rows = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", mondayAgo));
			
			Console.WriteLine("done!\r\n");
			Console.WriteLine(PrettyPrintAndPadWeekends(rows));
			Console.WriteLine("Press any key to close.");
			Console.ReadKey();
		}

		private static void Tracker_OnProgressChanged()
		{
			Console.Write(".");
		}

		/// <summary>
		///  Temporary weekend padded row until perdier fix for this.. O_o
		/// </summary>
		/// <param name="minMaxes"></param>
		/// <returns></returns>
		private static string PrettyPrintAndPadWeekends(IReadOnlyList<MinMax> minMaxes)
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

			for (int i = 0; i < count; i++)
			{
				var minMax = minMaxes.ElementAt(i);
				PadAppend(minMax.Min.ToString("yyyy-MM-dd"));
				PadAppend(minMax.Min.DayOfWeek.ToString());
				PadAppend(minMax.Min.ToString("HH:mm"));
				PadAppend(minMax.Max.ToString("HH:mm"));
				sb.AppendLine();

				if (minMax.Min.DayOfWeek == DayOfWeek.Friday && i + 1 < count)
				{
					sb.AppendLine();
				}
			}

			return sb.ToString();
		}
	}
}
