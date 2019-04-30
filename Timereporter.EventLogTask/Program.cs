using System;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;
using SimpleInjector;
using Timereporter.EventLogTask.Proxies;
using System.Diagnostics;
using Timereporter.Core;
using ConsoleTables;
using System.Collections.Generic;

namespace Timereporter.EventLogTask
{
	class Program
	{
		private static Lazy<Container> container = new Lazy<Container>(() => new Container());

		static void Main(string[] args)
		{
			RegisterServices(container.Value);

			var tracker = container.Value.GetInstance<EventLogTracker>();
			tracker.OnProgressChanged += Tracker_OnProgressChanged;
			var dateTimeValueFactory = container.Value.GetInstance<IDateTimeValueFactory>();
			Date from = WorkdayHelper.GetThreeMondaysAgo(dateTimeValueFactory.LocalToday());
			Date to = dateTimeValueFactory.LocalToday();
			var minMaxes = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", from, to, fill: true));

			Console.WriteLine("done!\r\n");
			Console.WriteLine(PrintConsoleTable(minMaxes));
			Console.WriteLine("Press any key to close.");
			Console.ReadKey();
		}

		private static void Tracker_OnProgressChanged()
		{
			Console.Write(".");
		}

		private static void RegisterServices(Container container)
		{
			container.Register<EventLogTracker>();
			container.Register<IEventLogProxy, EventLogProxy>(Lifestyle.Transient);
			container.Register(EventLogFactory, Lifestyle.Transient);
			container.Register<IDateTimeValueFactory, DateTimeValueFactory>(Lifestyle.Transient);
		}

		private static Func<EventLog> EventLogFactory()
		{
			return () => new EventLog();
		}

		private static string PrintConsoleTable(Dictionary<string, MinMax> minMaxes)
		{
			var table = new ConsoleTable("DATE", "DAY OF WEEK", "ARRIVAL", "LEAVE");

			foreach(var v in minMaxes.Values)
			{
				var min = v.Min.Match(some: m => m.ToString("HH:mm"), none: () => "-");
				var max = v.Max.Match(some: m => m.ToString("HH:mm"), none: () => "-");
				table.AddRow(v.Date, v.DayOfWeek, min, max);
			}

			return table.ToMinimalString();
		}
	}
}
