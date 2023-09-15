using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class updatedChatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatCourseBank");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "Chats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseID",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ChatCourseBank",
                columns: table => new
                {
                    ChatsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CoursesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatCourseBank", x => new { x.ChatsId, x.CoursesId });
                    table.ForeignKey(
                        name: "FK_ChatCourseBank_Chats_ChatsId",
                        column: x => x.ChatsId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatCourseBank_CourseBanks_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "CourseBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatCourseBank_CoursesId",
                table: "ChatCourseBank",
                column: "CoursesId");
        }
    }
}
