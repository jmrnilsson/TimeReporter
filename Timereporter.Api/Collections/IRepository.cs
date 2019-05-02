using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optional;

namespace Timereporter.Api.Collections
{
	public interface IRepository<T, TKey>
	{
		void Save(T value);
		Option<T> FindByKey(TKey key);
	}
}
