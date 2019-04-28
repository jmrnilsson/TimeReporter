using System;
using System.Diagnostics;
using Timereporter.EventLogTask;

namespace Timereporter.EventLogTask
{
	public delegate EventLog EventLogFactory();

	// TODO: Change to TinyIoC
	public sealed class ObjectFactory
	{
		private static readonly Lazy<ObjectFactory> lazy = new Lazy<ObjectFactory>(() => new ObjectFactory());
		public static ObjectFactory Instance { get { return lazy.Value; } }

		private EventLogFactory createEventLog;
		private Func<IDateTimeValueFactory> createDateTimeNow;
		private Func<EventLogTracker> createEventLogTracker;

		private ObjectFactory()
		{
			createEventLog = () => new EventLog();
			createDateTimeNow = () => new DateTimeValueFactory();
			createEventLogTracker = () => new EventLogTracker(createDateTimeNow());
		}

		public EventLog EventLog()
		{
			return createEventLog();
		}
		public IDateTimeValueFactory DateTimeValueFactory()
		{
			return createDateTimeNow();
		}
		public EventLogTracker EventLogTracker()
		{
			return createEventLogTracker();
		}
	}
}
