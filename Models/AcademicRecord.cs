using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAcademicManagementSystem.Models
{
	public class AcademicRecord
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RecordId { get; set; }

		[ForeignKey("Student")]
		[Required(ErrorMessage = "StudentId is required")]
		public int StudentId { get; set; }

		[ForeignKey("Course")]
		[Required(ErrorMessage = "CourseId is required")]
        public int CourseId { get; set; }

		[Required(ErrorMessage="Grade is required")]
        [StringLength(1, ErrorMessage = "Grade must be exactly 1 character")]
        [RegularExpression(@"^[A-F]$", ErrorMessage = "Grade must be a single letter from A to F")]
        public string Grade { get; set; } = string.Empty;

		[Required(ErrorMessage = "Semester is required")]
        [StringLength(1, ErrorMessage = "Semester must be a single digit")]
        public string Semester { get; set; } = string.Empty;

		public Student? Student { get; set; }
		public Course? Course { get; set; }
	}
}
