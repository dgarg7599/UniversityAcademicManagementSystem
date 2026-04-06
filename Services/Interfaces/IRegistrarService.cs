using UniversityAcademicManagementSystem.Models;

namespace UniversityAcademicManagementSystem.Services.Interfaces
{
    public interface IRegistrarService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<bool> AddCourseAsync(Course model);
        Task<bool> UpdateCourseAsync(Course model);
        Task<bool> DeleteCourseAsync(int id);

    }
}
