using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.App.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SmartCampusDbContext _db;

        public UserService(UserManager<AppUser> userManager, SmartCampusDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<AppUser> CreateUserAsync(CreateUserDto dto)
        {
            if (dto.Role == "Admin")
                throw new Exception("Admins cannot be created through this endpoint.");

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, dto.Role);

            if (dto.Role == "Student")
            {
                await _db.Students.AddAsync(new Student
                {
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    StudentCode = dto.StudentCode ?? $"S{user.Id}",
                    UserId = user.Id,
                    DepartmentId = dto.DepartmentId ?? throw new Exception("DepartmentId required for Student")
                });
            }
            else if (dto.Role == "Instructor")
            {
                await _db.Instructors.AddAsync(new Instructor
                {
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    UserId = user.Id,
                    DepartmentId = dto.DepartmentId ?? throw new Exception("DepartmentId required for Instructor")
                });
            }

            await _db.SaveChangesAsync();
            return user;

        }
    }
}
