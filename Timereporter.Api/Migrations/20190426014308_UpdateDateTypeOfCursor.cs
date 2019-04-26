using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class UpdateDateTypeOfCursor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Position",
                table: "Cursors",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Position",
                table: "Cursors",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
