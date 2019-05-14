using Event = Timereporter.Core.Models.Event;
using NodaTime;

namespace Timereporter.Api.Collections
{
	public interface IEventPersistentLog : IPersistentLog<(Instant, Instant), (long, long), Event> { }
}
