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
            try
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
            catch
            {
                return false;
            }
        }

        public async Task<List<User>> GetAllStaffAsync()
        {
            try
            {
                return await context.Users
                    .Where(u => u.Role == Role.Faculty || u.Role == Role.Registrar)
                    .ToListAsync();
            }
            catch
            {
                return new List<User>();
            }
        }

        public async Task<bool> RegisterStaffAsync(User model)
        {
            try
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
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = model.Role,
                    Department = model.Role == Role.Registrar
                        ? "Management"
                        : model.Department
                };

                context.Users.Add(newStaff);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateStaffAsync(User model)
        {
            try
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
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                existingUser.Role = model.Role;

                existingUser.Department = model.Role == Role.Registrar
                    ? "Management"
                    : model.Department;

                context.Users.Update(existingUser);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}