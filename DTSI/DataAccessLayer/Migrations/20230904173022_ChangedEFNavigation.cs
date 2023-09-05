using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class ChangedEFNavigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Students_StudentRegNo",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_StudentRegNo",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "StudentRegNo",
                table: "AppUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AppUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_AppUsers_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "StudentRegNo",
                table: "AppUsers",
                type: "nvarchar(13)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_StudentRegNo",
                table: "AppUsers",
                column: "StudentRegNo");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Students_StudentRegNo",
                table: "AppUsers",
                column: "StudentRegNo",
                principalTable: "Students",
                principalColumn: "RegNo");
        }
    }
}
