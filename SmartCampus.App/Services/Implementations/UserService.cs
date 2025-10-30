using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;


namespace SmartCampus.App.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }
        public async Task<AppUser> CreateUserAsync(CreateUserDto dto)
        {
            if (dto.Role == "Admin")
                throw new Exception("Admins cannot be created through this endpoint.");

            var user = new AppUser
            {
                UserName = dto.UserName,
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
                await _userRepository.CreateStudent(new Student
                {
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    StudentCode = dto.StudentCode ?? $"S{user.Id}",
                    ContactNumber = dto.ContactNumber,
                    Level = dto.Level,
                    UserId = user.Id,
                    DepartmentId = dto.DepartmentId ?? throw new Exception("DepartmentId required for Student")
                });
            }
            else if (dto.Role == "Instructor")
            {
                await _userRepository.CreateInstructor(new Instructor
                {
                    FullName = $"{dto.FirstName} {dto.LastName}",
                    ContactNumber= dto.ContactNumber,
                    UserId = user.Id,
                    DepartmentId = dto.DepartmentId ?? throw new Exception("DepartmentId required for Instructor")
                });
            }
            return user;

        }
    }
}
