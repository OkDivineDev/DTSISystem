using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class EFUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Assignments_AssignmentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_AssignmentId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "StudentAssignments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentRegNo = table.Column<string>(type: "nvarchar(13)", nullable: false),
                    AssignmentID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssignments_Assignments_AssignmentID",
                        column: x => x.AssignmentID,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssignments_Students_StudentRegNo",
                        column: x => x.StudentRegNo,
                        principalTable: "Students",
                        principalColumn: "RegNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignments_AssignmentID",
                table: "StudentAssignments",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignments_StudentRegNo",
                table: "StudentAssignments",
                column: "StudentRegNo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAssignments");

            migrationBuilder.AddColumn<string>(
                name: "AssignmentId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AssignmentId",
                table: "Students",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Assignments_AssignmentId",
                table: "Students",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
