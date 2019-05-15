using Optional;
using Timereporter.Core.Models;

namespace Timereporter.Api.Collections
{
	public interface ICursorRepository
	{
		Option<Cursor> Find(string cursorType);
		void Save(Cursor cursor);
	}
}