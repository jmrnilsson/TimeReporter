using Newtonsoft.Json;
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
		public static void PostEvents(Dictionary<string, Time> times)
		{
			var chunks = times.Chunkmap().ToList();

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				foreach (var chunk in chunks)
				{
					Post(client, chunk);
				}
			}
		}

		public static void PostEvent(Event @event)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				Post(client, new List<Event>() { @event });
			}
		}

		private static void Post(HttpClient client, List<Event> events)
		{
			// Previous url http://localhost:53762/api/events

			var json = JsonConvert.SerializeObject(new Events { Events_ = events });
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var result = client.PostAsync("http://localhost:53762/api/events", content).Result;

			if (!result.IsSuccessStatusCode)
			{
				throw new ApplicationException("esent rest");
			}
		}
	}
}
