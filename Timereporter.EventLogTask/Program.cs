using System;

namespace Timereporter.EventLogTask
{
	class Program
	{
		static void Main(string[] args)
		{
			var tracker = ObjectFactory.Instance.EventLogTracker();
			var dateTimeValueFactory = ObjectFactory.Instance.DateTimeValueFactory();
			var printableRows = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", dateTimeValueFactory.LocalNow()));

			Console.WriteLine("done!");

			foreach (var row in printableRows)
			{
				Console.WriteLine(row);
			}

			Console.WriteLine("Press any key to close.");
			Console.ReadKey();
		}
	}
}
