using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timereporter.Api.Models
{
	[Table("Events")]
	public class EventDo
	{
		public string Kind { get; set; }
		public long Timestamp { get; set; }
		[Required]
		public DateTime Added { get; set; }
		[Required]
		public DateTime Changed { get; set; }
	}
}
