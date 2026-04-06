using Microsoft.EntityFrameworkCore;
using UniversityAcademicManagementSystem.Data;
using UniversityAcademicManagementSystem.Models;
using UniversityAcademicManagementSystem.Services.Interfaces;

namespace UniversityAcademicManagementSystem.Services.Implementations
{
    public class UserService : IUserService
    {

        private readonly UniversityDbContext context;

        public UserService(UniversityDbContext context)
        {
            this.context = context;
        }


        public async Task<User?> LoginUserAsync(string email, string password)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            return user;
        }

        public async Task<bool> RegisterUserAsync(User model)
        {
            var exists = await context.Users.AnyAsync(u => u.Email == model.Email);
            if (exists) return false;


            var newUser = new User
            {
                Email = model.Email,
                Password = model.Password,
                Role = Role.Student
            };



            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            var newStudent = new Student
            {
                UserId = newUser.UserId,
                Email = model.Email
            };

            context.Students.Add(newStudent);
            await context.SaveChangesAsync();

            return true;
        }


        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            return user;
        }
    }
}
