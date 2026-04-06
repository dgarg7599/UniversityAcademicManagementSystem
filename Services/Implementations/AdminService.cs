using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Data;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UniversityDbContext context;

        public AdminService(UniversityDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetAllStaffAsync()
        {
            return await context.Users
            .Where(u => u.Role == Role.Faculty || u.Role == Role.Registrar)
            .ToListAsync();
        }

        public async Task<bool> RegisterStaffAsync(User model)
        {
            var exists = await context.Users.AnyAsync(u => u.Email == model.Email);
            if (exists) return false;

            if (model.Role != Role.Faculty && model.Role != Role.Registrar)
            {
                return false;
            }

            var newStaff = new User
            {
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            context.Users.Add(newStaff);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStaffAsync(User model)
        {
            var existingUser = await context.Users.FindAsync(model.UserId);
            if (existingUser == null) return false;

            var isEmailTaken = await context.Users.AnyAsync(u =>
                u.Email == model.Email && u.UserId != model.UserId);

            if (isEmailTaken)
            {
                return false;
            }

            existingUser.Email = model.Email;
            existingUser.Password = model.Password;
            existingUser.Role = model.Role;

            context.Users.Update(existingUser);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
