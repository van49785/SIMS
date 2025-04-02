using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CourseId { get; set; }

        public float? Grade { get; set; }
        public string Status { get; set; } = string.Empty; // "Passed" hoặc "Failed"
        public Course Course { get; set; }
        public ApplicationUser  Student { get; set; }

    }

}
