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
using Timereporter.Core.Tasks.EventLogTracker;

namespace Timereporter.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Contains("logs"))
			{
				EventLogTracker.Print("^ESENT$", "Application");
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
