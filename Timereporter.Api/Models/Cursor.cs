using System;
using System.ComponentModel.DataAnnotations;

namespace Timereporter.Api.Models
{
	public class Cursor
	{
		// https://docs.microsoft.com/en-us/ef/core/modeling/keys
		[Key]
		[StringLength(25)]
		public string CursorType { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public long Position { get; set; }
	}
}
