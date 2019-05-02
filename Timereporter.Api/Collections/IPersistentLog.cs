using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timereporter.Api.Collections
{
	public interface IPersistentLog<TArg, TArg0, TArg1, TArg2, T>
	{
		void AddRange(IEnumerable<T> iterable);
		T[] FindBy(TArg args);
		T[] FindBy(TArg0 args);
		T[] FindBy(TArg1 args);
		T[] FindBy(TArg2 args);
	}

	public interface IPersistentLog<TArg, TArg0, TArg1, T>
	{
		void AddRange(IEnumerable<T> iterable);
		T[] FindBy(TArg args);
		T[] FindBy(TArg0 args);
		T[] FindBy(TArg1 args);
	}
	public interface IPersistentLog<TArg, TArg0, T>
	{
		void AddRange(IEnumerable<T> iterable);
		T[] FindBy(TArg args);
		T[] FindBy(TArg0 args);
	}
	public interface IPersistentLog<TArg, T>
	{
		void AddRange(IEnumerable<T> iterable);
		T[] FindBy(TArg args);
	}
}
