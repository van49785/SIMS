using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required]
        [StringLength(50)]
        public string CourseName { get; set; } = string.Empty;  // Tên khóa học
        [Required]
        public int Credits { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        // Quan hệ với Enrollment (Một khóa học có thể có nhiều sinh viên)
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseInstructor> CourseInstructors { get; set; }
    }

}
