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
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetCourseById(int id)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course?> AddCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> UpdateCourse(Course course)
        {
            var courseExist = await _context.Courses.FindAsync(course.CourseId);
            if (courseExist == null)
                return null;

            _context.Entry(courseExist).CurrentValues.SetValues(course);
            await _context.SaveChangesAsync();
            return courseExist;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            // Check for related exams asynchronously
            var hasExams = await _context.Exams.AnyAsync(e => e.CourseId == id);
            if (hasExams)
            {
                throw new InvalidOperationException("Cannot delete course with existing exams.");
            }

            // Check for active enrollments
            var hasActiveEnrollments = await _context.Enrollments
                .AnyAsync(e => e.CourseId == id && e.Status == "Enrolled");
            if (hasActiveEnrollments)
            {
                throw new InvalidOperationException("Cannot delete course with active enrollments.");
            }

            var courseExist = await _context.Courses.FindAsync(id);
            if (courseExist == null)
                return false;

            _context.Courses.Remove(courseExist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Course>> GetCoursesByInstructorId(int instructorId)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .Where(c => c.InstructorId == instructorId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetEnrollmentStudentsByCourseID(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(en => en.Student)
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<IEnumerable<Course>> GetAllCoursesByDepartmentID(int departmentId)
        {
            return await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .Where(c => c.DepartmentId == departmentId)
                .AsNoTracking()
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
            var enrollmentCourse = await _context.Enrollments.FindAsync(enrollmentId);
            if (enrollmentCourse == null)
                return false;

            _context.Enrollments.Remove(enrollmentCourse);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Department)
                .Where(e => e.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentByStudentIdAndCourseId(int studentId, int courseId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task<bool> IsStudentEnrolledInCourse(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }
    }
}