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
            return await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<bool> IsProfileCompleteAsync(string email)
        {
            var s = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (s == null) return false;
            return !string.IsNullOrEmpty(s.Department) && !string.IsNullOrEmpty(s.ContactNumber);
        }

        public async Task<bool> UpdateStudentProfileAsync(Student student)
        {
            _context.Students.Update(student);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
