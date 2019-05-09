using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timereporter.Api.Models
{
	[Table("Workdays")]
	public class WorkdayDo
	{
		[Key]
		[StringLength(10)]
		public string Date { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public int? ArrivalMilliseconds { get; set; }
		public int? DepartureMilliseconds { get; set; }
		public int? BreakMilliseconds { get; set; }	
		public long ConcurrencyToken { get; set; }
	}
}
