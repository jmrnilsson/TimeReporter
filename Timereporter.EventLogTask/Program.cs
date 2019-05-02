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

namespace Timereporter.EventLogTask
{

	public static class ApiClient
	{
		public static void PostEvents(Dictionary<string, Time> times)
		{
			var chunks = times.Chunkmap().ToList();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				foreach (var chunk in chunks)
				{
					Post(client, chunk);
				}
			}
		}

		private static void Post(HttpClient client, List<Event> events)
		{
			// Previous url http://localhost:53762/api/events

			var json = JsonConvert.SerializeObject(new Events { Events_ = events });
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

			var result = client.PostAsync("http://localhost:53762/api/events", content).Result;

			if (!result.IsSuccessStatusCode)
			{
				throw new ApplicationException("esent rest");
			}
		}
	}

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
			Dictionary<string, Time> minMaxes = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", from, to, fill: true));


			Console.WriteLine("done!\r\n");
			Console.WriteLine(PrintConsoleTable(minMaxes));
			Console.WriteLine("Synchronizing results..");
			ApiClient.PostEvents(minMaxes);
			Console.WriteLine("done!\r\n");
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
