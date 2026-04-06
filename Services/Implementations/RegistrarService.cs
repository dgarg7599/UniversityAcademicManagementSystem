using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Data;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Services.Implementations
{
    public class RegistrarService : IRegistrarService
    {
        private readonly UniversityDbContext context;

        public RegistrarService(UniversityDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await context.Courses
                .OrderBy(c => c.Department)
                .ThenBy(c => c.SemesterOffered)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await context.Courses.FindAsync(id);
        }

        public async Task<bool> AddCourseAsync(Course model)
        {
            var exists = await context.Courses.AnyAsync(c =>
                c.CourseName.ToLower() == model.CourseName.ToLower() &&
                c.Department == model.Department &&
                c.SemesterOffered == model.SemesterOffered);

            if (exists) return false;

            var newCourse = new Course
            {
                CourseName = model.CourseName,
                Credits = model.Credits,
                Department = model.Department,
                SemesterOffered = model.SemesterOffered
            };

            context.Courses.Add(newCourse);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCourseAsync(Course model)
        {
            var existingCourse = await context.Courses.FindAsync(model.CourseId);
            if (existingCourse == null) return false;

            var isDuplicate = await context.Courses.AnyAsync(c =>
                c.CourseId != model.CourseId &&
                c.CourseName.ToLower() == model.CourseName.ToLower() &&
                c.Department == model.Department &&
                c.SemesterOffered == model.SemesterOffered);

            if (isDuplicate) return false;

            existingCourse.CourseName = model.CourseName;
            existingCourse.Credits = model.Credits;
            existingCourse.Department = model.Department;
            existingCourse.SemesterOffered = model.SemesterOffered;

            context.Courses.Update(existingCourse);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course != null)
            {
                context.Courses.Remove(course);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}