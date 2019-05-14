using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timereporter.Api.Models
{
	[Table("Workdays")]
	public class WorkdayDo
	{
		[StringLength(10)]
		public string Date { get; set; }
		[StringLength(25)]
		public string Kind { get; set; }
		[Required]
		public DateTime Added { get; set; }
		[Required]
		public DateTime Changed { get; set; }
		public long? Arrival { get; set; }
		public long? Departure { get; set; }
		public long? Break { get; set; }
		[Required]
		public string HashCode { get; set; }
	}
}
