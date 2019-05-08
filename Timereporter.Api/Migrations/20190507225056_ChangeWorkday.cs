using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class ChangeWorkday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureSeconds",
                table: "Workdays",
                newName: "DepartureMilliseconds");

            migrationBuilder.RenameColumn(
                name: "BreakSeconds",
                table: "Workdays",
                newName: "BreakMilliseconds");

            migrationBuilder.RenameColumn(
                name: "ArrivalSeconds",
                table: "Workdays",
                newName: "ArrivalMilliseconds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureMilliseconds",
                table: "Workdays",
                newName: "DepartureSeconds");

            migrationBuilder.RenameColumn(
                name: "BreakMilliseconds",
                table: "Workdays",
                newName: "BreakSeconds");

            migrationBuilder.RenameColumn(
                name: "ArrivalMilliseconds",
                table: "Workdays",
                newName: "ArrivalSeconds");
        }
    }
}
