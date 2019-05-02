using System;
using System.ComponentModel.DataAnnotations;

namespace Timereporter.Api.Models
{
	public class WorkdayComment
	{
		public int WorkdayCommentId { get; set; }
		[Required]
		public DateTime Added { get; set; }
		[Required]
		public string Comment { get; set; }

		public int WorkdayId { get; set; }
		public Workday Workday { get; set; }
	}
}
