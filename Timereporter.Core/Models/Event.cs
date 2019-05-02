using NodaTime;
using System;
using System.Collections.Generic;

namespace Timereporter.Core.Models
{
	public class Event
	{
		public string Kind { get; set; }
		public long Timestamp { get; set; }

		public Event() { }

		public Event(string kind, Instant instant)
		{
			Kind = kind;
			Timestamp = instant.ToUnixTimeMilliseconds();
		}

		public Event(string kind, long timestamp)
		{
			Kind = kind;
			Timestamp = timestamp;
		}
	}

	public class Events
	{
		public List<Event> Events_ { get; set; }
	}
}
