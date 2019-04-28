using Optional;
using System;
using System.Diagnostics;
using Timereporter.Core;
using Timereporter.EventLogTask.Proxies;

namespace Timereporter.EventLogTask
{
	public delegate IEventLogProxy EventLogFactory();
	public delegate EventLogTracker EventLogTrackerFactory(IEventLogProxy eventLogFactory);

	// TODO: Change to TinyIoC, this is a mess !O_o!
	public sealed class ObjectFactory
	{
		// TODO: reset readonly or remove completely
		private static Lazy<ObjectFactory> lazy = new Lazy<ObjectFactory>(() => new ObjectFactory());
		public static ObjectFactory Instance { get { return lazy.Value; } }

		private EventLogFactory eventLogFactory;
		private Func<IDateTimeValueFactory> dateTimeValueFactory;
		private Func<EventLogTracker> eventLogTrackerFactory;

		private ObjectFactory(EventLogFactory eventLogFactory = null, EventLogTrackerFactory eventLogTrackerFactory = null)
		{
			this.eventLogFactory = eventLogFactory ?? new EventLogFactory(() => new EventLogProxy(new EventLog()));
			dateTimeValueFactory = () => new DateTimeValueFactory();
			this.eventLogTrackerFactory = eventLogTrackerFactory.SomeNotNull().Match<Func<EventLogTracker>>
			(
				some: (elt) => () => elt(this.eventLogFactory()),
				none: () => () => new EventLogTracker(dateTimeValueFactory(), this.eventLogFactory())
			);
		}

		// TODO: Public-accessor, temporarily I guess, until TinyIoC is in place.
		public void Set(EventLogFactory eventLogFactory)
		{
			// Singletons and not lazy methods going to be tonne of problems here. Evaluation of each delegate needed.
			this.eventLogFactory = eventLogFactory;
		}

		// TODO: Public-accessor, remove method later on.
		public void Reset()
		{
			lazy = new Lazy<ObjectFactory>(() => new ObjectFactory());
		}

		public IEventLogProxy EventLog()
		{
			return eventLogFactory();
		}
		public IDateTimeValueFactory DateTimeValueFactory()
		{
			return dateTimeValueFactory();
		}
		public EventLogTracker EventLogTracker()
		{
			return eventLogTrackerFactory();
		}
	}
}
