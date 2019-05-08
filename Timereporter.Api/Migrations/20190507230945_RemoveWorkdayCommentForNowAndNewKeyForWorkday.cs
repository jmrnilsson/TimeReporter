using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Timereporter.Api.Migrations
{
    public partial class RemoveWorkdayCommentForNowAndNewKeyForWorkday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkdayComments");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Workdays",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 8);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Workdays",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.CreateTable(
                name: "WorkdayComments",
                columns: table => new
                {
                    WorkdayCommentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Added = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    WorkdayDate = table.Column<string>(nullable: true),
                    WorkdayId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkdayComments", x => x.WorkdayCommentId);
                    table.ForeignKey(
                        name: "FK_WorkdayComments_Workdays_WorkdayDate",
                        column: x => x.WorkdayDate,
                        principalTable: "Workdays",
                        principalColumn: "Date",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkdayComments_WorkdayDate",
                table: "WorkdayComments",
                column: "WorkdayDate");
        }
    }
}
