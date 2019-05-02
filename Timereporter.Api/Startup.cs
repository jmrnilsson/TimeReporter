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

namespace Timereporter.Api
{
	public delegate DatabaseContext DatabaseContextFactoryDelegate();
	public class Startup
	{
		const string conn = @"Data Source=DESKTOP-0G9PV2L;Initial Catalog=Timereporter;Integrated Security=True";
		// const string conn = @"Server=localhost\SQLEXPRESS;Database=Timereporter;Trusted_Connection=True;";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			// var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;ConnectRetryCount=0";
			services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(conn));
			services.AddSingleton<DatabaseContextFactoryDelegate>(DatabaseContextFactory);
			services.AddScoped<ICursors, Cursors>();
			services.AddScoped<IEvents, Events>();
			services.AddScoped<Workdays>();
		}

		private static DatabaseContext DatabaseContextFactory()
		{
			var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
			optionsBuilder.UseSqlServer(conn);
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
	}
}
