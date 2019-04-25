using System;

namespace Timereporter.Api.Collections
{
	public class Event : Dto
	{
		public Event(string kind, double moment)
			:base ()
		{
			Kind = kind;
			Moment = moment.UnixTimeStampToDateTime();
		}

		public Event(DateTime added, string kind, DateTime moment)
			 : base(added)
		{
			Kind = kind;
			Moment = moment;
		}

		public string Kind { get; }
		public DateTime Moment { get; }
	}
}
