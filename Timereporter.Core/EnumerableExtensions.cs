using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timereporter.Core
{
	public static class TapListShim
	{
		public static List<T> ToListAndTap<T>(this IEnumerable<T> iterable, Action<int, T> tapFunc, Predicate<T> filter = null)
		{
			List<T> list = new List<T>();
			// int i = 0;  // Sadly i think we'd have to do it this way to not enumrate the
			foreach (var tuple in iterable.Select((item, i) => (Item: item, Index:i)))
			{
				tapFunc(tuple.Index, tuple.Item);
				if (filter == null || filter(tuple.Item))
				{
					list.Add(tuple.Item);
				}
			}
			return list;
		}
	}
}
