using System;

namespace Timereporter.Api.Models
{
	public class Event
	{
		public int EventId { get; set; }
		public DateTime Added { get; set; }
		public string Kind { get; set; }
		public DateTime Moment { get; set; }
	}
}
