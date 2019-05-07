using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class WorkdayModificationV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalHours",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "BreakHours",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "DepartureHours",
                table: "Workdays");

            migrationBuilder.AddColumn<int>(
                name: "ArrivalSeconds",
                table: "Workdays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakSeconds",
                table: "Workdays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartureSeconds",
                table: "Workdays",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalSeconds",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "BreakSeconds",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "DepartureSeconds",
                table: "Workdays");

            migrationBuilder.AddColumn<float>(
                name: "ArrivalHours",
                table: "Workdays",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "BreakHours",
                table: "Workdays",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DepartureHours",
                table: "Workdays",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
