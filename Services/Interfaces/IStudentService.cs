using UniversityAcademicManagementSystem.Models;

namespace UniversityAcademicManagementSystem.Services.Interfaces
{
    public interface IStudentService
    {
        Task<Student?> GetStudentByEmailAsync(string email);
        Task<bool> UpdateStudentProfileAsync(Student student);
        Task<bool> IsProfileCompleteAsync(string email);
    }
}
