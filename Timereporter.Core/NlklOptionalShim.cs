using System;

namespace Timereporter.Api.Controllers
{
	public static class NlklOptionalShim
	{
		/// <summary>
		/// Shim of one kind of Match-verb from Nlkls Optional type for C#
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="T0"></typeparam>
		/// <param name="someOrNone"></param>
		/// <param name="some"></param>
		/// <param name="none"></param>
		/// <returns></returns>
		public static T0 Match<T, T0>(this T someOrNone, Func<T, T0> some, Func<T0> none) where T : class
		{
			if (someOrNone == default(T))
			{
				return none();
			}

			return some(someOrNone);
		}
	}
}
