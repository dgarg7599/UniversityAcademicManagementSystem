using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAcademicManagementSystem.Models
{
	public class Grade
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int GradeId { get; set; }

		[ForeignKey("Student")]
		[Required(ErrorMessage = "StudentId is required")]
		public int StudentId { get; set; }

		[ForeignKey("Course")]
		[Required(ErrorMessage = "CourseId is required")]
		public int CourseId { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [StringLength(1, ErrorMessage = "Grade must be exactly 1 character")]
        [RegularExpression(@"^[A-F]$", ErrorMessage = "Grade must be a single letter from A to F")]
        public string GradeValue { get; set; } = string.Empty;

		[StringLength(255, ErrorMessage = "Remarks cannot exceed 255 characters")]
		public string? Remarks { get; set; }

		public Student? Student { get; set; }
		public Course? Course { get; set; }
	}
}
