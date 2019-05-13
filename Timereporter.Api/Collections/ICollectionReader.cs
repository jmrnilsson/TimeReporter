using System.Collections.Generic;

namespace Timereporter.Api.Collections
{
	public interface ICollectionReader<T, TExpression>
	{
		List<T> Find(TExpression expression);
	}
}
