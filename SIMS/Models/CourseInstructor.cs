using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class CourseInstructor
    {
        [Key]
        public int CourseInstructorId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int InstructorId { get; set; }
        public ApplicationUser Instructor { get; set; }
    }
}
