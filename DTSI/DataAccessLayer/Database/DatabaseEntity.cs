using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Database
{
    public class DatabaseEntity : DbContext
    {
        public DatabaseEntity(DbContextOptions<DatabaseEntity> options) : base(options)
        {
        }

        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<CourseBank> CourseBanks { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }

        public virtual DbSet<Admission> Admissions { get; set; }
        public virtual DbSet<AcademicSession> Sessions { get; set; }

        public virtual DbSet<HyperLink> HyperLinks { get; set; }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<CourseAllocation> CourseAllocations { get; set; }
    }
}