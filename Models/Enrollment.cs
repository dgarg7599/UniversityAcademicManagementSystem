using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAcademicManagementSystem.Models
{
	public class Enrollment
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int EnrollmentId { get; set; }

		[ForeignKey("Student")]
		[Required(ErrorMessage = "StudentId is required")]
		public int StudentId { get; set; }

		[ForeignKey("Course")]
		[Required(ErrorMessage = "CourseId is required")]
		public int CourseId { get; set; }

		[Required(ErrorMessage = "Enrollment status must be specified")]
		public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.ENROLLED;

		public Student? Student { get; set; }
		public Course? Course { get; set; }
	}
}
