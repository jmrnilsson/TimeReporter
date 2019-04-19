using System.Collections.Generic;

namespace Timereporter.Models
{
	public class GridMetadata
	{
		public List<Workday> Workdays { get; set; }

		public HashSet<int> WeekendIndices { get; set; }
		
	}
}
