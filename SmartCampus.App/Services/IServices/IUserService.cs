using SmartCampus.App.DTOs;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.IServices
{
    public interface IUserService
    {
        Task<AppUser> CreateUserAsync(CreateUserDto dto);

        Task<IEnumerable<StudentDto>> GetAllStudents();
        Task<IEnumerable<InstructorDto>> GetAllInstructors();


        Task<StudentDto?> GetStudentById(int id);
        Task<InstructorDto?> GetInstructorById(int id);


        Task<UpdateStudentDto?> UpdateStudent( string email, UpdateStudentDto studentdto);
        Task<UpdateInstructorDto?> UpdateInstructor(string email, UpdateInstructorDto instructordto);
        Task<UpdateStudentNumberDto?> UpdateSudentContactNumber(int id, string PhoneNumber);


        Task<bool> DeleteStudent(int id);
        Task<bool> DeleteInstructor(int id,int replaceId);
        
    }
}
