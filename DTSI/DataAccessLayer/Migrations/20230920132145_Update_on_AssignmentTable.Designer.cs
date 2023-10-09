﻿// <auto-generated />
using System;
using DataAccessLayer.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(DatabaseEntity))]
    [Migration("20230920132145_Update_on_AssignmentTable")]
    partial class Update_on_AssignmentTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DataAccessLayer.Models.AcademicSession", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Admission", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EntryMode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntryYear")
                        .HasColumnType("int");

                    b.Property<int>("GraduationYear")
                        .HasColumnType("int");

                    b.Property<string>("JambRegNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Session")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Admissions");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Assignment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("CourseID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("LecturerID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Questions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseID");

                    b.HasIndex("LecturerID");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Chat", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseAllocation", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Approved")
                        .HasColumnType("bit");

                    b.Property<string>("CourseID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LecturerID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Semester")
                        .HasColumnType("int");

                    b.Property<string>("Session")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CourseID");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("LecturerID");

                    b.ToTable("CourseAllocations");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseBank", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentID");

                    b.ToTable("CourseBanks");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseOutLine", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("OutLine")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SN")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseOutLines");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Department", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("HODUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("DataAccessLayer.Models.HyperLink", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("HyperLinks");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Inventory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Lecturer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Lecturers");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Student", b =>
                {
                    b.Property<string>("RegNo")
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<string>("DepartmentID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("EntryMode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntryYear")
                        .HasColumnType("int");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GraduationYear")
                        .HasColumnType("int");

                    b.Property<string>("JambRegNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Passport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RegNo");

                    b.HasIndex("DepartmentID");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Students");
                });

            modelBuilder.Entity("DataAccessLayer.Models.StudentAssignment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AssignmentID")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StudentRegNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(13)");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentID");

                    b.HasIndex("StudentRegNo");

                    b.ToTable("StudentAssignments");
                });

            modelBuilder.Entity("DataAccessLayer.Models.UserInterface", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("DepartmentLecturer", b =>
                {
                    b.Property<string>("DepartmentsId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LecturersId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("DepartmentsId", "LecturersId");

                    b.HasIndex("LecturersId");

                    b.ToTable("DepartmentLecturer");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Admission", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Department", "Department")
                        .WithMany("Admissions")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Assignment", b =>
                {
                    b.HasOne("DataAccessLayer.Models.CourseBank", "Course")
                        .WithMany("Assignments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.Lecturer", "Lecturer")
                        .WithMany("Assignments")
                        .HasForeignKey("LecturerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Lecturer");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseAllocation", b =>
                {
                    b.HasOne("DataAccessLayer.Models.CourseBank", "Course")
                        .WithMany("CourseAllocations")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.Department", "Department")
                        .WithMany("CourseAllocations")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.Lecturer", "Lecturer")
                        .WithMany("CourseAllocations")
                        .HasForeignKey("LecturerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Department");

                    b.Navigation("Lecturer");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseBank", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Department", "Department")
                        .WithMany("Courses")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseOutLine", b =>
                {
                    b.HasOne("DataAccessLayer.Models.CourseBank", "Course")
                        .WithMany("CourseOutLines")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Lecturer", b =>
                {
                    b.HasOne("DataAccessLayer.Models.UserInterface", "User")
                        .WithMany("Lecturers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Student", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Department", "Department")
                        .WithMany("Students")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.UserInterface", "User")
                        .WithOne("Student")
                        .HasForeignKey("DataAccessLayer.Models.Student", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DataAccessLayer.Models.StudentAssignment", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Assignment", "Assignment")
                        .WithMany("StudentAssignments")
                        .HasForeignKey("AssignmentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.Student", "Student")
                        .WithMany("StudentAssignments")
                        .HasForeignKey("StudentRegNo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("DepartmentLecturer", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Department", null)
                        .WithMany()
                        .HasForeignKey("DepartmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Models.Lecturer", null)
                        .WithMany()
                        .HasForeignKey("LecturersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataAccessLayer.Models.Assignment", b =>
                {
                    b.Navigation("StudentAssignments");
                });

            modelBuilder.Entity("DataAccessLayer.Models.CourseBank", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("CourseAllocations");

                    b.Navigation("CourseOutLines");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Department", b =>
                {
                    b.Navigation("Admissions");

                    b.Navigation("CourseAllocations");

                    b.Navigation("Courses");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Lecturer", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("CourseAllocations");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Student", b =>
                {
                    b.Navigation("StudentAssignments");
                });

            modelBuilder.Entity("DataAccessLayer.Models.UserInterface", b =>
                {
                    b.Navigation("Lecturers");

                    b.Navigation("Student");
                });
#pragma warning restore 612, 618
        }
    }
}
