﻿using Newtonsoft.Json;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Timereporter.Core.Models;

namespace Timereporter.Core
{

	public static class ApiClient
	{
		public static IEnumerable<WorkdayDetailsDto> GetData(int year, int month)
		{
			using (var client = new ApiHttpClient())
			{
				var dateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
				var dtz = dateTimeZone.Id.Replace("/", "_");
				var uri = $"http://localhost:53762/api/workday/{year}/{month}/{dtz}";
				var response = client.Get<Workdays>(uri);
				return response.Item2.Map<IEnumerable<WorkdayDetailsDto>>(wd => wd.List).ValueOr(Enumerable.Empty<WorkdayDetailsDto>());
			}
		}
	}

	public class EventEqualityComparer : IEqualityComparer<Event>
	{
		public bool Equals(Event x, Event y)
		{
			return x.Kind == y.Kind && x.Timestamp == y.Timestamp;
		}

		public int GetHashCode(Event obj)
		{
			return obj.Kind.GetHashCode() ^ obj.Timestamp.GetHashCode();
		}
	}
}
