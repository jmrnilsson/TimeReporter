using Newtonsoft.Json;
using Optional;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Timereporter.Core
{
	public class ApiHttpClient : IDisposable
	{
		private readonly HttpClientHandler httpClienthandler;
		private readonly HttpClient httpClient;
		private const int defaultTransactionLength = 500;

		public ApiHttpClient()
		{
			httpClienthandler = new HttpClientHandler();
			httpClient = new HttpClient(httpClienthandler, false);
			httpClient.DefaultRequestHeaders.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

		}

		public IEnumerable<HttpResponseMessage> Post<T, TValue>
		(
			string url,
			List<T> list,
			Func<List<T>, TValue> postChunkValueFactory = null
		) where T : new() where TValue : new()
		{
			var chunked = Chunk(list);
			
			foreach(var chunk in chunked)
			{
				string json = JsonConvert.SerializeObject(postChunkValueFactory(list));
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				yield return httpClient.PostAsync(url, content).Result;
			}
		}

		public IEnumerable<HttpResponseMessage> Post<T> (string url, List<T> list) where T : new()
		{
			var chunked = Chunk(list);

			foreach (var chunk in chunked)
			{
				string json = JsonConvert.SerializeObject(list);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				yield return httpClient.PostAsync(url, content).Result;
			}
		}


		public static IEnumerable<List<T>> Chunk<T>(List<T> items, int length = defaultTransactionLength)
		{
			for (int i = 0; i < items.Count; i += length)
			{
				yield return items.GetRange(i, Math.Min(length, items.Count - i));
			}
		}

		public (HttpStatusCode, Option<T>) Get<T>(string url) where T : new()
		{
			var response = httpClient.GetAsync(url).Result;

			if (!response.IsSuccessStatusCode)
			{
				return (response.StatusCode, Option.None<T>());
			}

			var json = response.Content.ReadAsStringAsync().Result;
			return (response.StatusCode, JsonConvert.DeserializeObject<T>(json).Some());
		}

		public HttpStatusCode Get(string url)
		{
			var response = httpClient.GetAsync(url).Result;
			return response.StatusCode;
		}


		public void Dispose()
		{
			httpClienthandler.Dispose();
			httpClient.Dispose();
		}
	}
}
