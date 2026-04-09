using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Data;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly UniversityDbContext _context;
        public StudentService(UniversityDbContext context) => _context = context;

        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            try
            {
                return await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsProfileCompleteAsync(string email)
        {
            try
            {
                var s = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
                if (s == null) return false;
                return !string.IsNullOrEmpty(s.Department) && !string.IsNullOrEmpty(s.ContactNumber);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStudentProfileAsync(Student student)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == student.Email);
                if (user != null)
                {
                    user.Department = student.Department;
                    _context.Users.Update(user);
                }
                _context.Students.Update(student);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EnrollInCourseAsync(int studentId, int courseId)
        {
            try
            {
                var existingEnrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

                if (existingEnrollment != null)
                {
                    if (existingEnrollment.EnrollmentStatus == EnrollmentStatus.ENROLLED)
                    {
                        return false;
                    }

                    existingEnrollment.EnrollmentStatus = EnrollmentStatus.ENROLLED;
                    _context.Enrollments.Update(existingEnrollment);
                }
                else
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        EnrollmentStatus = EnrollmentStatus.ENROLLED
                    };
                    _context.Enrollments.Add(enrollment);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DropCourseAsync(int enrollmentId)
        {
            try
            {
                var enrollment = await _context.Enrollments.FindAsync(enrollmentId);

                if (enrollment == null) return false;

                enrollment.EnrollmentStatus = EnrollmentStatus.DROPPED;

                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Course>> GetAvailableCoursesAsync(string dept, int studentId)
        {
            try
            {
                var activeEnrolledIds = await _context.Enrollments
                    .Where(e => e.StudentId == studentId && e.EnrollmentStatus == EnrollmentStatus.ENROLLED)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                return await _context.Courses
                    .Where(c => c.Department == dept && !activeEnrolledIds.Contains(c.CourseId))
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Course>();
            }
        }

        public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(int studentId)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Course)
                    .Where(e => e.StudentId == studentId &&
                                e.EnrollmentStatus == EnrollmentStatus.ENROLLED)
                    .ToListAsync();
            }
            catch
            {
                return new List<Enrollment>();
            }
        }

        public async Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId)
        {
            try
            {
                return await _context.Grades
                    .Include(g => g.Course)
                    .Where(g => g.StudentId == studentId)
                    .OrderByDescending(g => g.GradeId)
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Grade>();
            }
        }

        public async Task<(double CGPA, int TotalCourses)> CalculateCGPAAsync(int studentId)
        {
            try
            {
                var grades = await _context.Grades
                    .Where(g => g.StudentId == studentId)
                    .ToListAsync();

                int totalCourses = grades.Count;
                if (totalCourses == 0) return (0, 0);

                double totalPoints = 0;

                foreach (var g in grades)
                {
                    totalPoints += g.GradeValue.ToUpper() switch
                    {
                        "A" => 10,
                        "B" => 8,
                        "C" => 6,
                        "D" => 4,
                        "E" => 2,
                        _ => 0
                    };
                }

                double cgpa = Math.Round(totalPoints / totalCourses, 2);
                return (cgpa, totalCourses);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}