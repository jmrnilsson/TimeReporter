using System;
using Timereporter.Core.Collections;
using Timereporter.Core.Models;
using SimpleInjector;
using Timereporter.EventLogTask.Proxies;
using System.Diagnostics;
using Timereporter.Core;
using ConsoleTables;
using System.Collections.Generic;
using NodaTime.TimeZones;
using NodaTime;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using Timereporter.Core.Reducers;

namespace Timereporter.EventLogTask
{

	class Program
	{
		private static Lazy<Container> container = new Lazy<Container>(() => new Container());
		private const string windowsEventLogExpression = "^ESENT$";

		static void Main(string[] args)
		{
			RegisterServices(container.Value);

			Console.Write("Checking api availability..");
			bool apiOnline = ApiClient.Ping();
			Console.WriteLine("done!");

			DateTimeZone dtz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

			IWindowsEventLogReader windowsEventLogReader = container.Value.GetInstance<IWindowsEventLogReader>();
			windowsEventLogReader.OnProgressChanged += Tracker_OnProgressChanged;
			var dateTimeValueFactory = container.Value.GetInstance<IDateTimeValueFactory>();
			LocalDate from = new LocalDate(2019, 1, 1);
			LocalDate to = SystemClock.Instance.GetCurrentInstant().InZone(dtz).Date;

			Console.Write("Scanning windows log for expression '{0}'..", windowsEventLogExpression);
			var query = new EventLogQuery(windowsEventLogExpression, "Application", from, to, fill: true);
			var entries = windowsEventLogReader.ReadAll(query);
			Dictionary<string, Time> minMaxes = entries.ToSummarizedWorkdays(query.From, query.To, query.Pattern, query.Fill);
			Console.WriteLine("done!");

			Console.WriteLine(PrintConsoleTable(minMaxes));

			if (apiOnline)
			{
				Console.Write("Synchronizing log entries..");
				ApiClient.PostEvents(entries, dtz);
				Console.WriteLine("done!");
				Console.Write("Synchronizing similified arrivals and depatures..");
				ApiClient.PostEvents(minMaxes);
				Console.WriteLine("done!");
				Console.WriteLine("Press any key to close.");
			}
			else
			{
				Console.WriteLine("Skipping synchronization..");
			}

			Console.ReadKey();
		}



		private static void Tracker_OnProgressChanged()
		{
			Console.Write(".");
		}

		private static void RegisterServices(Container container)
		{
			container.Register<IWindowsEventLogReader, WindowsEventLogReader>();
			container.Register<IEventLogProxy, EventLogProxy>(Lifestyle.Transient);
			container.Register(EventLogFactory, Lifestyle.Transient); 
			container.Register<IDateTimeValueFactory, DateTimeValueFactory>(Lifestyle.Transient);
		}

		private static Func<EventLog> EventLogFactory()
		{
			return () => new EventLog();
		}

		private static string PrintConsoleTable(Dictionary<string, Time> minMaxes)
		{
			var tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			var table = new ConsoleTable("DATE", "DAY OF WEEK", "ARRIVAL", "LEAVE");

			string LocalTime(Instant instant)
			{
				return instant.InZone(tz).LocalDateTime.ToDateTimeUnspecified().ToString("HH:mm");
			}

			foreach(var v in minMaxes.Values)
			{
				var min = v.Min.Match(some: LocalTime, none: () => "-");
				var max = v.Max.Match(some: LocalTime, none: () => "-");
				table.AddRow(v.Date, v.DayOfWeek, min, max);
			}

			return table.ToMinimalString();
		}
	}
}
