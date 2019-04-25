using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Timereporter.Api.Models
{
	// https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/existing-db
	// https://docs.microsoft.com/en-us/ef/core/
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options)
			: base(options)
		{ }

		public DbSet<Event> Events { get; set; }
		public DbSet<Workday> Workdays { get; set; }
		public DbSet<WorkdayComment> WorkdayComments { get; set; }
	}

	public class Event
	{
		public int EventId { get; set; }
		public DateTime Added { get; set; }
		public string Kind { get; set; }
		public DateTime Moment { get; set; }
	}

	public class Workday
	{
		public int WorkdayId { get; set; }
		public DateTime Added { get; set; }
		public DateTime Changed { get; set; }
		public string Date { get; set; }
		public DateTime Arrival { get; set; }
		public DateTime Departure { get; set; }
		public int BreakDurationSeconds { get; set; }
	}

	public class WorkdayComment
	{
		public int WorkdayCommentId { get; set; }
		public DateTime Added { get; set; }
		public string Comment { get; set; }

		public int WorkdayId { get; set; }
		public Workday Workday { get; set; }
	}
}
