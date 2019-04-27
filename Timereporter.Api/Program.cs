using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Timereporter.Api.CoreTasks.EventLogReader;

namespace Timereporter.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Contains("logs"))
			{
				var tracker = ObjectFactory.Instance.EventLogTracker();
				var dateTimeValueFactory = ObjectFactory.Instance.DateTimeValueFactory();
				var printableRows = tracker.FindBy(new EventLogQuery("^ESENT$", "Application", dateTimeValueFactory.LocalNow()));
			}
			else
			{
				CreateWebHostBuilder(args).Build().Run();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();

	}
}
