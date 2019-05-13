using System.Collections.Generic;

namespace Timereporter.Core.Models
{
	public interface IEventLogProxy
	{
		string Log { get; set; }
		IEnumerable<IEventLogEntryProxy> Entries { get; }
	}
}
