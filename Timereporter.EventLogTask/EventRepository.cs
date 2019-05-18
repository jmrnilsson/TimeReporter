using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timereporter.Core;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	public class EventRepository
	{
		public static List<Event> ToList(Dictionary<string, Time> times)
		{
			List<Event> events = new List<Event>();

			foreach (var t in times)
			{
				// TODO: Replace with Some(Action<>) me thinks. Also use workday reporting instead of events. Also push events rather.
				t.Value.Source.MatchSome(source =>
				{
					t.Value.Min.MatchSome(some: min => events.Add(new Event($"{source}_MIN", min)));
					t.Value.Max.MatchSome(some: max => events.Add(new Event($"{source}_MAX", max)));
				});
			}
			return events;
		}


		public void PostEvents(Dictionary<string, Time> times)
		{
			List<Event> list = ToList(times);

			using (var client = new ApiHttpClient())
			{
				client.Post("http://localhost:53762/api/events", list);
			}
		}

		public void PostEvent(Event @event)
		{
			using (var client = new ApiHttpClient())
			{
				client.Post("http://localhost:53762/api/events", new List<Event>() { @event });
			}
		}

		public bool Ping()
		{
			using (var client = new ApiHttpClient())
			{
				return (int)client.Get("http://localhost:53762/api/ping") > 299;
			}
		}

		public void PostEvents(List<IEventLogEntryProxy> entries, DateTimeZone dtz)
		{
			List<Event> events = entries.MapToEvents(dtz).Distinct(new EventEqualityComparer()).ToList();

			using (var client = new ApiHttpClient())
			{
				client.Post("http://localhost:53762/api/events", events, list => new Events { Events_ = list });
			}
		}

	}
}
