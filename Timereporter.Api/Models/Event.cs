using System;
using System.ComponentModel.DataAnnotations;

namespace Timereporter.Api.Models
{
	public class Event
	{
		public string Kind { get; set; }
		public long Timestamp { get; set; }
		[Required]
		public DateTime Added { get; set; }
		[Required]
		public DateTime Changed { get; set; }
	}
}
