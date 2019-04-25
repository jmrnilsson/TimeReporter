using System;

namespace Timereporter.Api.Collections
{
	public abstract class Dto
	{
		public Dto()
		{
			Added = DateTime.UtcNow;
		}
		public Dto(DateTime added)
		{
			Added = added;
		}
		public DateTime Added { get; }
	}
}
