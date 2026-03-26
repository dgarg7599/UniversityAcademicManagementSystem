using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Models;
namespace UniversityAcademicManagementSystem.Data
{
	public class UniversityDbContext : DbContext
	{

		public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) {}

		public DbSet<Student> Students { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Enrollment> Enrollments { get; set; }
		public DbSet<Grade> Grades { get; set; }
		public DbSet<AcademicRecord> AcademicRecords { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseName);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.EnrollmentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Grade>()
                .Property(g => g.GradeValue)
                .HasMaxLength(5);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.EnrollmentStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            base.OnModelCreating(modelBuilder);
        }
    }
}
