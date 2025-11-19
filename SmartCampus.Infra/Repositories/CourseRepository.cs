using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.Infra.Repositories
{
    public class CourseRepository : ICourseRepository
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
            var course = await _context.Courses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return false;

            if (course.IsDeleted)
                throw new InvalidOperationException("Course is already deleted.");

            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PermanentlyDeleteCourse(int id)
        {
            var course = await _context.Courses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
                return false;

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

    

       
        /// Get active enrollment count for a course

        public async Task<int> GetActiveEnrollmentCountByCourseId(int courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Enrolled")
                .CountAsync();
        }

        /// Get student's total credits for current semester
     
        public async Task<int> GetStudentCurrentSemesterCredits(int studentId, DateTime semesterStartDate)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId &&
                           e.Status == "Enrolled" &&
                           e.EnrollmentDate >= semesterStartDate)
                .SumAsync(e => e.CreditHours);
        }

        /// Get student's total credits for current academic year
    
        public async Task<int> GetStudentCurrentYearCredits(int studentId, DateTime yearStartDate)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId &&
                           e.Status == "Enrolled" &&
                           e.EnrollmentDate >= yearStartDate)
                .SumAsync(e => e.CreditHours);
        }

     
        /// Get instructor's active course count
   
        public async Task<int> GetInstructorActiveCourseCount(int instructorId)
        {
            return await _context.Courses
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .CountAsync();
        }

        /// Get instructor's total teaching credit hours
     
        public async Task<int> GetInstructorTotalCreditHours(int instructorId)
        {
            return await _context.Courses
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .SumAsync(c => c.Credits);
        }

   
        /// Get completed courses for a student (for prerequisites check)
    
        public async Task<List<string>> GetStudentCompletedCourseCodes(int studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == "Completed")
                .Select(e => e.CourseCode)
                .ToListAsync();
        }


        /// Get courses available for a specific department
        /// Only returns courses that belong to the student's department
      
        public async Task<IEnumerable<Course>> GetCoursesByDepartmentForStudent(int departmentId)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Department)
                .Where(c => c.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();
        }

               /// Check if a course belongs to a specific department
 
        public async Task<bool> IsCourseBelongsToDepartment(int courseId, int departmentId)
        {
            return await _context.Courses
                .AnyAsync(c => c.CourseId == courseId && c.DepartmentId == departmentId);
        }
    }
}
