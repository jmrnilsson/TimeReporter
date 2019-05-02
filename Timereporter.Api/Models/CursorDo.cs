using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timereporter.Api.Models
{
	[Table("Cursors")]
	public class CursorDo
	{
		// https://docs.microsoft.com/en-us/ef/core/modeling/keys
		[Key]
		[StringLength(25)]
		public string CursorType { get; set; }
		[Required]
		public DateTime Added { get; set; }
		[Required]
		public DateTime Changed { get; set; }
		[Required]
		public long Position { get; set; }
	}
}
