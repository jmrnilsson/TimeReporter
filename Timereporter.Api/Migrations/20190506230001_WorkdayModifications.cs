using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class WorkdayModifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arrival",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "BreakDurationSeconds",
                table: "Workdays");

            migrationBuilder.DropColumn(
                name: "Departure",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "Arrival",
                table: "Workdays",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "BreakDurationSeconds",
                table: "Workdays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Departure",
                table: "Workdays",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
