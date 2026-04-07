using UniversityAcademicManagementSystem.Models;

namespace UniversityAcademicManagementSystem.Services.Interfaces
{
    public interface IStudentService
    {
        Task<Student?> GetStudentByEmailAsync(string email);
        Task<bool> UpdateStudentProfileAsync(Student student);
        Task<bool> IsProfileCompleteAsync(string email);
        Task<bool> EnrollInCourseAsync(int studentId, int courseId);
        Task<bool> DropCourseAsync(int enrollmentId);
        Task<IEnumerable<Course>> GetAvailableCoursesAsync(string dept, int studentId);
        Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(int studentId);
        Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId);
        Task<(double CGPA, int TotalCourses)> CalculateCGPAAsync(int studentId);
    }
}
