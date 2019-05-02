using Cursor = Timereporter.Core.Models.Cursor;

namespace Timereporter.Api.Collections
{
	public interface ICursorRepository : IRepository<Cursor, string> { }
}
