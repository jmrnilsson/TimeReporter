using System;

namespace Timereporter.Core.Models
{
	public class Event
	{
		public Event(string kind, double moment)
			:base ()
		{
			Kind = kind ?? throw new ArgumentException(nameof(kind));
			Moment = moment.UnixTimeStampToDateTime();
			Added = DateTime.UtcNow;
		}

		public Event(DateTime added, string kind, DateTime moment)
		{
			Kind = kind ?? throw new ArgumentException(nameof(kind));
			Moment = moment;
			Added = added;
		}

		public string Kind { get; }
		public DateTime Moment { get; }
		public DateTime Added { get; }
	}
}
