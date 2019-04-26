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
		public DbSet<Cursor> Cursors { get; set; }
	}
}
