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

        public async Task<UpdateStudentDto?> UpdateStudent(string email ,UpdateStudentDto studentDto)
        {
            var exist= await _userRepository.GetStudentByEmail(email);
            if (exist == null)
                return null;

            exist.FullName = $"{studentDto.FirstName} {studentDto.LastName}".Trim();
            exist.ContactNumber = studentDto.ContactNumber;
           
            exist.Level = studentDto.Level;
            exist.DepartmentId = studentDto.DepartmentId;
            exist.UpdatedAt= DateTime.UtcNow;

            var updatedStudent = await _userRepository.UpdateStudent(exist);
            var user = await _userRepository.GetUserByEmail(email);
  
            
            user.UserName=studentDto.UserName;
            user.FirstName = studentDto.FirstName;
            user.LastName = studentDto.LastName;
            user.PhoneNumber = studentDto.ContactNumber;
            user.UpdatedAt = DateTime.UtcNow;
            user.PhoneNumberConfirmed=user.PhoneNumber != null;

            
            var updatedUser = await _userRepository.UpdateUser(user);
            var nameParts = updatedStudent.FullName?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
            return new UpdateStudentDto
            {
                UserName = studentDto.UserName,
                FirstName = firstName,
                LastName = lastName,
                ContactNumber = updatedStudent.ContactNumber,
              
                Level = updatedStudent.Level,
                DepartmentId = updatedStudent.DepartmentId
            };
        }

        public async Task<UpdateInstructorDto?> UpdateInstructor(string email, UpdateInstructorDto instructorDto)
        {
            var exist = await _userRepository.GetInstructorByEmail( email);
            if (exist == null)
                return null;
            
            exist.FullName = $"{instructorDto.FirstName} {instructorDto.LastName}".Trim();
            exist.ContactNumber = instructorDto.ContactNumber;
            exist.DepartmentId = instructorDto.DepartmentId;
            exist.UpdatedAt = DateTime.UtcNow;

            var updatedInstructor = await _userRepository.UpdateInstructor(exist);
            var user = await _userRepository.GetUserByEmail(email);


            user.UserName = instructorDto.UserName;
            user.FirstName = instructorDto.FirstName;
            user.LastName = instructorDto.LastName;
            user.PhoneNumber = instructorDto.ContactNumber;
            user.UpdatedAt = DateTime.UtcNow;
            user.PhoneNumberConfirmed = user.PhoneNumber != null;


            var updatedUser = await _userRepository.UpdateUser(user);

            var nameParts = updatedInstructor.FullName?.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
            return new UpdateInstructorDto
            {
                UserName = instructorDto.UserName,
                FirstName = firstName,
                LastName = lastName,
                ContactNumber = updatedInstructor.ContactNumber,
                DepartmentId = updatedInstructor.DepartmentId
            };
        }

        public async Task<bool> DeleteStudent(int id)
        {
            var student = await _userRepository.GetStudentById(id);
            if (student == null)
                return false;

            return await _userRepository.DeleteStudent(id);
        }

        public async Task<bool> DeleteInstructor(int id,int replaceId)
        {
            var instructor = await _userRepository.GetInstructorById(id);
            if (instructor == null)
                return false;

            return await _userRepository.DeleteInstructor(id, replaceId);
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudents()
        {
            var students = await _userRepository.GetAllStudents();
            return students.Select(s => new StudentDto
            {
                FullName = s.FullName ?? "Unknown",
                Email = s.User?.Email ?? "No Email",
                ContactNumber = s.ContactNumber ?? "There is no Contact Number",
                StudentCode = s.StudentCode,
                Level = s.Level ?? "No Level Yet",
                DepartmentId = s.DepartmentId ?? 0
            });
        }

        public async Task<IEnumerable<InstructorDto>> GetAllInstructors()
        {
            var instructors = await _userRepository.GetAllInstructors();
            return instructors.Select(i => new InstructorDto
            {
                FullName = i.FullName,
                Email = i.User?.Email?? "No Email",
                ContactNumber = i.ContactNumber ?? "There is no Contact Number",
                DepartmentId = i.DepartmentId ?? 0
            });
        }

        public async Task<UpdateStudentNumberDto?> UpdateSudentContactNumber(int id, string phoneNum)
        {
            var student = await _userRepository.GetStudentById(id);
            if (student == null) return null;

            student.ContactNumber = phoneNum;
            await _userRepository.UpdateSudentContactNumber(id,phoneNum);
            return new UpdateStudentNumberDto
            {
                FullName = student.FullName,
                StudentCode = student.StudentCode,
                ContactNumber = student.ContactNumber
            };
        }

        public async Task<StudentDto?> GetStudentById(int id)
        {
            var student = await _userRepository.GetStudentById(id);
            return new StudentDto
            {
                FullName = student.FullName ,
                Email = student.User?.Email ?? "No Email Yet",
                StudentCode = student.StudentCode,
                ContactNumber = student.ContactNumber,
                Level = student.Level,
                DepartmentId = student.DepartmentId
            };
        }
        public async Task<InstructorDto?> GetInstructorById(int id)
        {
            var instructor = await _userRepository.GetInstructorById(id);
            return new InstructorDto
            {
                FullName = instructor.FullName,
                Email = instructor.User?.Email ?? "No Email Yet",              
                ContactNumber = instructor.ContactNumber,
                DepartmentId = instructor.DepartmentId
            };
        }


    }
}
