using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class updatedModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserInterfaceUserId",
                table: "Lecturers");

            migrationBuilder.DropIndex(
                name: "IX_Lecturers_UserInterfaceUserId",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "UserInterfaceUserId",
                table: "Lecturers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Lecturers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_UserId",
                table: "Lecturers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserId",
                table: "Lecturers",
                column: "UserId",
                principalTable: "UserInterfaces",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserId",
                table: "Lecturers");

            migrationBuilder.DropIndex(
                name: "IX_Lecturers_UserId",
                table: "Lecturers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Lecturers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserInterfaceUserId",
                table: "Lecturers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_UserInterfaceUserId",
                table: "Lecturers",
                column: "UserInterfaceUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserInterfaceUserId",
                table: "Lecturers",
                column: "UserInterfaceUserId",
                principalTable: "UserInterfaces",
                principalColumn: "UserId");
        }
    }
}
