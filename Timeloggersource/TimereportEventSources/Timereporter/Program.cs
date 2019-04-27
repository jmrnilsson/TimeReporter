using System;
using System.Diagnostics;

namespace Timereporter
{
	class Program
	{
		static void Main(string[] args)
		{
			Read();
			Console.WriteLine("Press enter to close.");
			Console.ReadLine();
		}

		private static void Read()
		{
			int i;
			string eventLogName = "Application";

			EventLog eventLog = ObjectFactory.Instance.CreateEventLog();
			eventLog.Log = eventLogName;
			EventsTimeSource eventTimeSource = new EventsTimeSource();

			i = 0;
			foreach (EventLogEntry log in eventLog.Entries)
			{
				if (i % 1000 == 0) Console.Write(".");
				eventTimeSource.Add(log);
				i++;
			}

			Console.WriteLine("");
			i = 0;
			foreach (var item in eventTimeSource.GetMinMax())
			{
				Console.WriteLine(item);
				i++;
			}
		}
	}
}
