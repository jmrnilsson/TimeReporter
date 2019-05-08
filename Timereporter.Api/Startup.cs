using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Timereporter.Api.Models;
using Timereporter.Api.Collections;
using NodaTime.Serialization;
using NodaTime.Serialization.JsonNet;
using NodaTime;
using System.Text.RegularExpressions;
using Timereporter.Core;

namespace Timereporter.Api
{
	public delegate DatabaseContext DatabaseContextFactoryDelegate();
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvcCore()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
				.AddJsonFormatters(jsonSerializerSettings => jsonSerializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

			// var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;ConnectRetryCount=0";
			services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(ResolveConnectionString()));
			services.AddSingleton<DatabaseContextFactoryDelegate>(DatabaseContextFactory);
			services.AddScoped<ICursorRepository, CursorRepository>();
			services.AddScoped<IEventLog, EventPersistentLog>();
			services.AddScoped<WorkdayRepository>();
			services.AddScoped<WorkdaySummarizer>();
		}

		private static DatabaseContext DatabaseContextFactory()
		{
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
			optionsBuilder.UseSqlServer(ResolveConnectionString());
			return new DatabaseContext(optionsBuilder.Options);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMvc();
		}

		private static string ResolveConnectionString()
		{
			if (Regex.IsMatch(Environment.MachineName, "HFB$"))
			{
				return @"Server=localhost\SQLEXPRESS;Database=Timereporter;Trusted_Connection=True;";
			}
			return @"Data Source=DESKTOP-0G9PV2L;Initial Catalog=Timereporter;Integrated Security=True";
		}
	}
}
