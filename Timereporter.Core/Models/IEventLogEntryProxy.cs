using System;

namespace Timereporter.Core.Models
{
	public interface IEventLogEntryProxy
	{
		DateTime TimeWritten { get; }
		string Source { get; }
	}
}
