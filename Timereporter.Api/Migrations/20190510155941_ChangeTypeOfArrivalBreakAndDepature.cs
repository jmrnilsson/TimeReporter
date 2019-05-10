using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class ChangeTypeOfArrivalBreakAndDepature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalMilliseconds",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "BreakMilliseconds",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "DepartureMilliseconds",
                table: "Workdays");

            migrationBuilder.AddColumn<long>(
                name: "Arrival",
                table: "Workdays",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Break",
                table: "Workdays",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Departure",
                table: "Workdays",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arrival",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "Break",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "Departure",
                table: "Workdays");

            migrationBuilder.AddColumn<int>(
                name: "ArrivalMilliseconds",
                table: "Workdays",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMilliseconds",
                table: "Workdays",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartureMilliseconds",
                table: "Workdays",
                nullable: true);
        }
    }
}
