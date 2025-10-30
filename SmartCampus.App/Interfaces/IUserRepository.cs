using SmartCampus.Core.Entities;

namespace SmartCampus.App.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> CreateUser(AppUser user);
        Task<Student?> CreateStudent(Student student);
        Task<Instructor?> CreateInstructor(Instructor instructor);


    }
}
