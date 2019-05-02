using System;
using System.Collections.Generic;

namespace Timereporter.Core.Models
{
	public class Event
	{
		public string Kind { get; set; }
		public long Timestamp { get; set; }
	}

	public class Events
	{
		public List<Event> Events_ { get; set; }
	}
}
