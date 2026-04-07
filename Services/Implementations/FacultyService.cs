using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Data;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly UniversityDbContext _context;
        public FacultyService(UniversityDbContext context) => _context = context;

        public async Task<IEnumerable<Grade>> GetAllGradesAsync() =>
            await _context.Grades.Include(g => g.Student).Include(g => g.Course).OrderByDescending(g => g.GradeId).ToListAsync();

        public async Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(string dept) =>
            await _context.Courses.Where(c => c.Department == dept).ToListAsync();

        public async Task<IEnumerable<object>> GetEnrolledStudentsByCourseAsync(int courseId) =>
            await _context.Enrollments.Where(e => e.CourseId == courseId && e.EnrollmentStatus == EnrollmentStatus.ENROLLED)
                .Select(e => new { StudentId = e.Student.StudentId, Name = e.Student.Name }).ToListAsync();

        public async Task<bool> IsGradeAlreadyExists(int studentId, int courseId) =>
            await _context.Grades.AnyAsync(g => g.StudentId == studentId && g.CourseId == courseId);

        public async Task<bool> AddGradeAsync(Grade model)
        {
            try
            {
                _context.Grades.Add(model);
                var course = await _context.Courses.FindAsync(model.CourseId);
                _context.AcademicRecords.Add(new AcademicRecord
                {
                    StudentId = model.StudentId,
                    CourseId = model.CourseId,
                    Grade = model.GradeValue,
                    Semester = course.SemesterOffered.ToString()
                });
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<Grade> GetGradeByIdAsync(int id) => await _context.Grades.FindAsync(id);

        public async Task<bool> UpdateGradeAsync(Grade model)
        {
            try
            {
                var existingGrade = await _context.Grades.FindAsync(model.GradeId);
                if (existingGrade == null) return false;

                existingGrade.GradeValue = model.GradeValue;
                existingGrade.Remarks = model.Remarks;

                var record = await _context.AcademicRecords
                    .FirstOrDefaultAsync(r => r.StudentId == existingGrade.StudentId && r.CourseId == existingGrade.CourseId);

                if (record != null)
                {
                    record.Grade = model.GradeValue;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteGradeAsync(int id)
        {
            try
            {
                var grade = await _context.Grades.FindAsync(id);
                if (grade == null) return false;

                var record = await _context.AcademicRecords.FirstOrDefaultAsync(r => r.StudentId == grade.StudentId && r.CourseId == grade.CourseId);
                if (record != null) _context.AcademicRecords.Remove(record);

                _context.Grades.Remove(grade);
                return await _context.SaveChangesAsync() > 0;
            }
            catch { return false; }
        }
    }
}

