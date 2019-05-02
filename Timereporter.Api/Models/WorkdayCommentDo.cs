using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timereporter.Api.Models
{
	[Table("WorkdayComments")]
	public class WorkdayCommentDo
	{
		[Key]
		public int WorkdayCommentId { get; set; }
		[Required]
		public DateTime Added { get; set; }
		[Required]
		public string Comment { get; set; }

		public int WorkdayId { get; set; }
		public WorkdayDo Workday { get; set; }
	}
}
