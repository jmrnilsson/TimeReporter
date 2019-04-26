using System;

namespace Timereporter.Api.Entities

{
	public class Cursor
	{
		public Cursor(string cursorType, long position)
		{
			CursorType = cursorType ?? throw new ArgumentException(nameof(cursorType));
			Position = position;
			Changed = DateTime.UtcNow;
		}

		public Cursor(DateTime changed, string cursorType, long position)
		{
			CursorType = cursorType ?? throw new ArgumentException(nameof(cursorType));
			Position = position;
			Changed = changed;
		}

		public string CursorType { get; }
		public long Position { get; }
		public DateTime Changed { get; }
	}
}
