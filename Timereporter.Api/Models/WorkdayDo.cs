using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timereporter.Api.Models
{
	[Table("Workdays")]
	public class WorkdayDo
	{
		[Key]
		[StringLength(8)]
		public string Date { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public DateTime Arrival { get; set; }
		public DateTime Departure { get; set; }
		public int BreakDurationSeconds { get; set; }
	}
}
