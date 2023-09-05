using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class ChangedTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserId",
                table: "Lecturers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInterfaces_Students_StudentRegNo",
                table: "UserInterfaces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInterfaces",
                table: "UserInterfaces");

            migrationBuilder.RenameTable(
                name: "UserInterfaces",
                newName: "AppUsers");

            migrationBuilder.RenameIndex(
                name: "IX_UserInterfaces_StudentRegNo",
                table: "AppUsers",
                newName: "IX_AppUsers_StudentRegNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUsers",
                table: "AppUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Students_StudentRegNo",
                table: "AppUsers",
                column: "StudentRegNo",
                principalTable: "Students",
                principalColumn: "RegNo");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_AppUsers_UserId",
                table: "Lecturers",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Students_StudentRegNo",
                table: "AppUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_AppUsers_UserId",
                table: "Lecturers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUsers",
                table: "AppUsers");

            migrationBuilder.RenameTable(
                name: "AppUsers",
                newName: "UserInterfaces");

            migrationBuilder.RenameIndex(
                name: "IX_AppUsers_StudentRegNo",
                table: "UserInterfaces",
                newName: "IX_UserInterfaces_StudentRegNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInterfaces",
                table: "UserInterfaces",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserId",
                table: "Lecturers",
                column: "UserId",
                principalTable: "UserInterfaces",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInterfaces_Students_StudentRegNo",
                table: "UserInterfaces",
                column: "StudentRegNo",
                principalTable: "Students",
                principalColumn: "RegNo");
        }
    }
}
