using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; } = string.Empty;

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }

}
