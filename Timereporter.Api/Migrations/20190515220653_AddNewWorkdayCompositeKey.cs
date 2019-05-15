using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class AddNewWorkdayCompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays",
                columns: new[] { "Date", "Kind" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays",
                column: "Kind");
        }
    }
}
