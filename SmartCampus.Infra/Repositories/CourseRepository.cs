using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.Infra.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SmartCampusDbContext _context;


        public CourseRepository(SmartCampusDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetCourseById(int id)
        {
            return await _context.Courses.FindAsync(id);
        }
        public async Task<Course?> AddCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> UpdateCourse(Course course)
        {
            var courseExit = _context.Courses.FindAsync(course.CourseId);
            if (courseExit != null)
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                return course;
            }
            return null;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var hasExams = _context.Exams.Any(e => e.CourseId == id);
            if (hasExams)
            {
                throw new InvalidOperationException("Cannot delete course with existing exams.");
            }
            var courseExit = await _context.Courses.FindAsync(id);
            if (courseExit != null)
            {
                _context.Courses.Remove(courseExit);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<Course>> GetCoursesByInstructorId(int instructorId)
        {
            return await _context.Courses.Include(c => c.Instructor)
                                         .Include(c => c.Department)
                                         .Where(c => c.InstructorId == instructorId).ToListAsync();
        }

        public async Task<Course?> GetEnrollmentStudentsByCourseID(int CourseID)
        {
            return await _context.Courses
                .Where(e => e.CourseId == CourseID)
                .Include(e => e.Enrollments)
                .ThenInclude(en => en.Student)
                .FirstOrDefaultAsync(c => c.CourseId == CourseID);

        }

        public async Task<IEnumerable<Course>> GetAllCoursesByDepartmentID(int DepartmentId)
        {
            return await _context.Courses
                 .Include(c => c.Department)
                 .Where(c => c.DepartmentId == DepartmentId)
                 .ToListAsync();
        }

        public async Task<Enrollment?> AddEnrollCourse(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }


        public async Task<bool> RemoveEnrollCourse(int enrollmentId)
        {
            var enrollmentcouse = await _context.Enrollments.FindAsync(enrollmentId);
            if (enrollmentcouse != null)
            {
                _context.Enrollments.Remove(enrollmentcouse);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _context.Enrollments
                 .Include(c => c.Student)
                 .Include(c => c.Course)
                 .Where(e => e.StudentId == studentId)
                 .ToListAsync();
            return enrollments;
        }

        public async Task<IEnumerable<Enrollment?>> GetEnrollmentByStudentIdAndCourseId(int studentId, int courseId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId && e.CourseId == courseId)
                .ToListAsync();
            return enrollments;
        }


    }
}
