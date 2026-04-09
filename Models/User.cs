using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAcademicManagementSystem.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId {  get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(300)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage ="Role is required")]
        public Role Role { get; set; } = Role.Student;

        [MaxLength(100)]
        public string? Department { get; set; } = string.Empty;
    }
}
