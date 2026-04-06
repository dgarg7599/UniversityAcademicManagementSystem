using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace UniversityAcademicManagementSystem.Models
{
	public class Course
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CourseId { get; set; }

		[Required(ErrorMessage = "Course Name is required")]
		[StringLength(100)]
		public string CourseName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Credits are required")]
		[Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
		public int Credits { get; set; }

		[Required(ErrorMessage = "Department is required")]
		[StringLength(100)]
		public string Department { get; set; } = string.Empty;

		[Required(ErrorMessage = "Semester is required")]
		[StringLength(1, ErrorMessage = "Semester must be a single digit")]
		[Range(1, 8, ErrorMessage = "Semester must be between 1 and 8")]
		public string SemesterOffered { get; set; } = string.Empty;

		public ICollection<Enrollment>? Enrollments { get; set; }
		public ICollection<Grade>? Grades { get; set; }
		public ICollection<AcademicRecord>? AcademicRecords { get; set; }
	}
}
