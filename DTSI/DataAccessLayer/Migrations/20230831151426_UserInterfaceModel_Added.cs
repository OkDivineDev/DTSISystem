using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class UserInterfaceModel_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentID",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentID",
                table: "Lecturers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserInterfaceUserId",
                table: "Lecturers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentID",
                table: "CourseBanks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentID",
                table: "CourseAllocations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "Admissions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HODUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInterfaces",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentRegNo = table.Column<string>(type: "nvarchar(13)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterfaces", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserInterfaces_Students_StudentRegNo",
                        column: x => x.StudentRegNo,
                        principalTable: "Students",
                        principalColumn: "RegNo");
                });

            migrationBuilder.CreateTable(
                name: "DepartmentLecturer",
                columns: table => new
                {
                    DepartmentsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LecturersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentLecturer", x => new { x.DepartmentsId, x.LecturersId });
                    table.ForeignKey(
                        name: "FK_DepartmentLecturer_Department_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentLecturer_Lecturers_LecturersId",
                        column: x => x.LecturersId,
                        principalTable: "Lecturers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentID",
                table: "Students",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_UserInterfaceUserId",
                table: "Lecturers",
                column: "UserInterfaceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseBanks_DepartmentID",
                table: "CourseBanks",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAllocations_DepartmentID",
                table: "CourseAllocations",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_DepartmentId",
                table: "Admissions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentLecturer_LecturersId",
                table: "DepartmentLecturer",
                column: "LecturersId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInterfaces_StudentRegNo",
                table: "UserInterfaces",
                column: "StudentRegNo");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Department_DepartmentId",
                table: "Admissions",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAllocations_Department_DepartmentID",
                table: "CourseAllocations",
                column: "DepartmentID",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseBanks_Department_DepartmentID",
                table: "CourseBanks",
                column: "DepartmentID",
                principalTable: "Department",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserInterfaceUserId",
                table: "Lecturers",
                column: "UserInterfaceUserId",
                principalTable: "UserInterfaces",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Department_DepartmentID",
                table: "Students",
                column: "DepartmentID",
                principalTable: "Department",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Department_DepartmentId",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAllocations_Department_DepartmentID",
                table: "CourseAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseBanks_Department_DepartmentID",
                table: "CourseBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_UserInterfaces_UserInterfaceUserId",
                table: "Lecturers");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Department_DepartmentID",
                table: "Students");

            migrationBuilder.DropTable(
                name: "DepartmentLecturer");

            migrationBuilder.DropTable(
                name: "UserInterfaces");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropIndex(
                name: "IX_Students_DepartmentID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Lecturers_UserInterfaceUserId",
                table: "Lecturers");

            migrationBuilder.DropIndex(
                name: "IX_CourseBanks_DepartmentID",
                table: "CourseBanks");

            migrationBuilder.DropIndex(
                name: "IX_CourseAllocations_DepartmentID",
                table: "CourseAllocations");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_DepartmentId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "UserInterfaceUserId",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "CourseBanks");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Admissions");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentID",
                table: "CourseAllocations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
