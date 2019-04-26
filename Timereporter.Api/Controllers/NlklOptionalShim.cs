using System;

namespace Timereporter.Api.Controllers
{
	public static class NlklOptionalShim
	{
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
