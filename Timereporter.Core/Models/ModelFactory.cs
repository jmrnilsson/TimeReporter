using NodaTime;
using System;

namespace Timereporter.Core.Models
{
	public static class ModelFactory
	{
		public static Event MakeEvent(string kind, Instant instant)
		{
			return new Event
			{
				Kind = kind ?? throw new ArgumentException(nameof(kind)),
				Timestamp = instant.ToUnixTimeMilliseconds()
			};
		}


		public static Event MakeEvent(string kind, long timestamp)
		{
			return new Event
			{
				Kind = kind ?? throw new ArgumentException(nameof(kind)),
				Timestamp = timestamp
			};
		}
	}
}

