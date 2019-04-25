using System;

namespace Timereporter.Api.Collections
{
	public static class Int32Extensions
	{
		public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}
	}
}
