using System;
using System.Diagnostics;

namespace Timereporter
{
	public delegate EventLog EventLogFactory();
	public delegate DateTime DateTimeNowFactory();

	public sealed class ObjectFactory
	{
		private static readonly Lazy<ObjectFactory> lazy = new Lazy<ObjectFactory>(() => new ObjectFactory());
		public static ObjectFactory Instance { get { return lazy.Value; } }

		private EventLogFactory createEventLog;
		private DateTimeNowFactory createDateTimeNow;

		private ObjectFactory()
		{
			createEventLog = () => new EventLog();
			createDateTimeNow = () => DateTime.Now;
		}

		public EventLog CreateEventLog()
		{
			return createEventLog();
		}
		public DateTime CreateDateTimeNow()
		{
			return createDateTimeNow();
		}
	}
}
