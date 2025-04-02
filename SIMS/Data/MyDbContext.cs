using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIMS.Models;

namespace SIMS.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {}

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseInstructor> CourseInstructors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .IsRequired(false);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Course)
                .WithMany(c => c.CourseInstructors)
                .HasForeignKey(ci => ci.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseInstructor>()
                .HasOne(ci => ci.Instructor)
                .WithMany()
                .HasForeignKey(ci => ci.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}