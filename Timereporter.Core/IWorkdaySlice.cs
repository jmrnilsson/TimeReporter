using Optional;

namespace Timereporter.Core
{
	public interface IWorkdaySlice
	{
		string Date { get; }
		string Kind { get; }
		Option<long> Arrival { get; }
		Option<long> Departure { get; }
		Option<long> Break { get;  }
		string HashCode { get; }
	}
}
