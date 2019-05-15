using System.Collections.Generic;
using Timereporter.Core.Models;

namespace Timereporter.EventLogTask
{
	public interface IWindowsEventLogReader
	{
		event ProgressChanged OnProgressChanged;

		List<IEventLogEntryProxy> ReadAll(EventLogQuery query);
	}
}