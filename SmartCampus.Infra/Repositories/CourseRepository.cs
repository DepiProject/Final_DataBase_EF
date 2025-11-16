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
            // Query filter automatically excludes IsDeleted = true
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetCourseById(int id)
        {
            // Query filter automatically excludes IsDeleted = true
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<Course?> AddCourse(Course course)
        {
            course.IsDeleted = false;
            course.DeletedAt = null;
            course.DeletedBy = null;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> UpdateCourse(Course course)
        {
            var courseExist = await _context.Courses.FindAsync(course.CourseId);
            if (courseExist == null)
                return null;

            // Preserve soft delete status during update
            course.IsDeleted = courseExist.IsDeleted;
            course.DeletedAt = courseExist.DeletedAt;
            course.DeletedBy = courseExist.DeletedBy;
            course.UpdatedAt = DateTime.UtcNow;

            _context.Entry(courseExist).CurrentValues.SetValues(course);
            await _context.SaveChangesAsync();
            return courseExist;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            // Use IgnoreQueryFilters to find even deleted courses
            var course = await _context.Courses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return false;

            if (course.IsDeleted)
                throw new InvalidOperationException("Course is already deleted.");

            // Soft delete - just mark as deleted
            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;
            // Optionally: course.DeletedBy = currentUserId; // if you have user context

            await _context.SaveChangesAsync();
            return true;
        }

        // New method to permanently delete a course (use with caution)
        public async Task<bool> PermanentlyDeleteCourse(int id)
        {
            var course = await _context.Courses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return false;

            // Check for related data
            var hasExams = await _context.Exams
                .IgnoreQueryFilters()
                .AnyAsync(e => e.CourseId == id);

            if (hasExams)
                throw new InvalidOperationException("Cannot permanently delete course with existing exams.");

            var hasEnrollments = await _context.Enrollments
                .IgnoreQueryFilters()
                .AnyAsync(e => e.CourseId == id);

            if (hasEnrollments)
                throw new InvalidOperationException("Cannot permanently delete course with existing enrollments.");

            var hasAttendances = await _context.Attendances
                .IgnoreQueryFilters()
                .AnyAsync(a => a.CourseId == id);

            if (hasAttendances)
                throw new InvalidOperationException("Cannot permanently delete course with existing attendances.");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        // New method to restore a soft-deleted course
        public async Task<bool> RestoreCourse(int id)
        {
            var course = await _context.Courses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return false;

            if (!course.IsDeleted)
                throw new InvalidOperationException("Course is not deleted.");

            course.IsDeleted = false;
            course.DeletedAt = null;
            course.DeletedBy = null;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // New method to get all courses including deleted ones
        public async Task<IEnumerable<Course>> GetAllCoursesIncludingDeleted()
        {
            return await _context.Courses
                .IgnoreQueryFilters()
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .AsNoTracking()
                .ToListAsync();
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