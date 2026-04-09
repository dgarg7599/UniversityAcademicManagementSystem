using UniversityAcademicManagementSystem.Models;

namespace UniversityAcademicManagementSystem.Services.Interfaces
{
    public interface IFacultyService
    {
        Task<IEnumerable<Grade>> GetAllGradesAsync();
        Task<IEnumerable<Course>> GetCoursesByDepartmentAsync(string dept);
        Task<IEnumerable<object>> GetEnrolledStudentsByCourseAsync(int courseId);
        Task<bool> AddGradeAsync(Grade gradeModel, string faculty);
        Task<Grade> GetGradeByIdAsync(int id);
        Task<bool> UpdateGradeAsync(Grade model);
        Task<bool> DeleteGradeAsync(int id);
        Task<bool> IsGradeAlreadyExists(int studentId, int courseId);
        Task<IEnumerable<Grade>> GetGradesByDepartmentAsync(string dept);
    }
}
