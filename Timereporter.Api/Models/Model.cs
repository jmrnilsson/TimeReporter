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

		public DbSet<EventDo> Events { get; set; }
		public DbSet<WorkdayDo> Workdays { get; set; }
		public DbSet<WorkdayCommentDo> WorkdayComments { get; set; }
		public DbSet<CursorDo> Cursors { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<EventDo>()
				.HasKey(c => new { c.Timestamp, c.Kind });
		}
	}
}
