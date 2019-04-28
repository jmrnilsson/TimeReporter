using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class ChangePKWorkday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkdayComments_Workdays_WorkdayId",
                table: "WorkdayComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_WorkdayComments_WorkdayId",
                table: "WorkdayComments");

            migrationBuilder.DropColumn(
                name: "WorkdayId",
                table: "Workdays");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Workdays",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkdayDate",
                table: "WorkdayComments",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_WorkdayComments_WorkdayDate",
                table: "WorkdayComments",
                column: "WorkdayDate");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkdayComments_Workdays_WorkdayDate",
                table: "WorkdayComments",
                column: "WorkdayDate",
                principalTable: "Workdays",
                principalColumn: "Date",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkdayComments_Workdays_WorkdayDate",
                table: "WorkdayComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays");

            migrationBuilder.DropIndex(
                name: "IX_WorkdayComments_WorkdayDate",
                table: "WorkdayComments");

            migrationBuilder.DropColumn(
                name: "WorkdayDate",
                table: "WorkdayComments");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Workdays",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 8);

            migrationBuilder.AddColumn<int>(
                name: "WorkdayId",
                table: "Workdays",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workdays",
                table: "Workdays",
                column: "WorkdayId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkdayComments_WorkdayId",
                table: "WorkdayComments",
                column: "WorkdayId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkdayComments_Workdays_WorkdayId",
                table: "WorkdayComments",
                column: "WorkdayId",
                principalTable: "Workdays",
                principalColumn: "WorkdayId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
