using System;

namespace Timereporter.Api.Models
{
	public class WorkdayComment
	{
		public int WorkdayCommentId { get; set; }
		public DateTime Added { get; set; }
		public string Comment { get; set; }

		public int WorkdayId { get; set; }
		public Workday Workday { get; set; }
	}
}
