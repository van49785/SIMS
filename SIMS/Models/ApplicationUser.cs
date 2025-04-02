using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SIMS.Models
{
    public class ApplicationUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string UserCode { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } // Mật khẩu mã hóa
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        [Required]
        [StringLength(20)]
        public string Role { get; set; } // "Student", "Instructor", "Admin"
        public List<Enrollment> Enrollments { get; set; }
    }
}