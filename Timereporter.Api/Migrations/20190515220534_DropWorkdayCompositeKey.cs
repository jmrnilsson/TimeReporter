using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class DropWorkdayCompositeKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays");

            migrationBuilder.AlterColumn<int>(
                name: "Date",
                table: "Workdays",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays",
                column: "Kind");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Workdays",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays",
                columns: new[] { "Date", "Kind" });
        }
    }
}
