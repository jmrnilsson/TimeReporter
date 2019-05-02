using Microsoft.EntityFrameworkCore;

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
		public DbSet<Cursor> Cursors { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Event>()
				.HasKey(c => new { c.Timestamp, c.Kind });
		}
	}
}
