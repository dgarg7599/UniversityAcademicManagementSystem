using UniversityAcademicManagementSystem.Models;

namespace UniversityAcademicManagementSystem.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> RegisterStaffAsync(User model);

        Task<List<User>> GetAllStaffAsync();

        Task<bool> DeleteStaffAsync(int id);

        Task<bool> UpdateStaffAsync(User model);
    }
}
