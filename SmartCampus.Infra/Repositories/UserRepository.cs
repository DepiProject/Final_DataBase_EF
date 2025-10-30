using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;


namespace SmartCampus.Infra.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SmartCampusDbContext _context;

        public UserRepository(SmartCampusDbContext context)
        {
            _context = context;
        }
        public async Task<AppUser?> CreateUser(AppUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<Student?> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }
        public async Task<Instructor?> CreateInstructor(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            return instructor;
        }

    }
}
