using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace UniversityAcademicManagementSystem.Models
{
	public class Student
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int StudentId { get; set; }

		[Required(ErrorMessage = "UserId is required")]
		[ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Invalid format")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [StringLength(10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a 10-digit phone number without spaces or dashes.")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enrollment year is required")]
        [Range(2000, 2100, ErrorMessage = "Please enter a valid year between 2000 and 2100")]
        public int EnrollmentYear { get; set; }

        public User? User { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
		public ICollection<Grade>? Grades { get; set; }
		public ICollection<AcademicRecord>? AcademicRecords { get; set; }
	}
}
