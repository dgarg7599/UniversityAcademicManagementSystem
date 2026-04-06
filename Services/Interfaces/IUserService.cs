using UniversityAcademicManagementSystem.Models;

namespace UniversityAcademicManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(User model);
        Task<User?> LoginUserAsync(string email, string password);
        Task<User> GetUserByIdAsync(int id);
    }
}
