using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IUserRepository _userRepo;

        public CourseService(ICourseRepository courseRepo, IUserRepository userRepo)
        {
            _courseRepo = courseRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCourses()
        {
            var courses = await _courseRepo.GetAllCourses();
            return courses.Select(c => new CourseDTO
            {
                Name = c.Name,
                CreditHours = c.Credits,
                InstructorId = c.InstructorId
            });
        }

        public async Task<CourseDTO?> GetCourseById(int id)
        {
            var course = await _courseRepo.GetCourseById(id);
            if (course == null)
                return null;

            return new CourseDTO
            {
                Name = course.Name,
                CreditHours = course.Credits,
                InstructorId = course.InstructorId
            };
        }

        public async Task<CreateCourseDTO?> AddCourse(CreateCourseDTO courseDto)
        {
            // Validate instructor exists
            if (courseDto.InstructorId == 0)
                throw new ArgumentException("Instructor ID is required for creating a course.");

            var instructor = await _userRepo.GetInstructorById(courseDto.InstructorId);
            if (instructor == null)
                throw new InvalidOperationException($"Instructor with ID {courseDto.InstructorId} does not exist.");

            // Validate course code uniqueness (optional - handled by DB constraint too)
            var existingCourse = await _courseRepo.GetAllCourses();
            if (existingCourse.Any(c => c.CourseCode == courseDto.CourseCode))
                throw new InvalidOperationException($"Course code '{courseDto.CourseCode}' already exists.");

            var course = new Course
            {
                CourseCode = courseDto.CourseCode,
                Name = courseDto.Name,
                Credits = courseDto.CreditHours,
                InstructorId = courseDto.InstructorId,
                DepartmentId = courseDto.DepartmentId
            };

            await _courseRepo.AddCourse(course);
            return courseDto;
        }

        public async Task<CourseDTO?> UpdateCourse(int id, CourseDTO courseDto)
        {
            var courseExist = await _courseRepo.GetCourseById(id);
            if (courseExist == null)
                return null;

            // Validate new instructor exists
            var instructor = await _userRepo.GetInstructorById(courseDto.InstructorId);
            if (instructor == null)
                throw new InvalidOperationException($"Instructor with ID {courseDto.InstructorId} does not exist.");

            courseExist.Name = courseDto.Name;
            courseExist.Credits = courseDto.CreditHours;
            courseExist.InstructorId = courseDto.InstructorId;

            var updatedCourse = await _courseRepo.UpdateCourse(courseExist);
            if (updatedCourse == null)
                return null;

            return new CourseDTO
            {
                Name = updatedCourse.Name,
                CreditHours = updatedCourse.Credits,
                InstructorId = updatedCourse.InstructorId
            };
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var course = await _courseRepo.GetCourseById(id);
            if (course == null)
                return false;

            return await _courseRepo.DeleteCourse(id);
        }

        public async Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentStudentsByCourseID(int courseId)
        {
            var course = await _courseRepo.GetEnrollmentStudentsByCourseID(courseId);
            if (course == null || course.Enrollments == null)
                return new List<studentEnrollmentDTO>();

            return course.Enrollments.Select(e => new studentEnrollmentDTO
            {
                courseName = course.Name,
                courseCode = course.CourseCode,
                studentName = e.Student?.FullName ?? "Unknown",
                CreditHours = course.Credits,
                DepartmentName = course.Department?.Name ?? "Unknown"
            });
        }

        public async Task<IEnumerable<EnrollCourseDTO>> GetAllCoursesByDepartmentID(int departmentId)
        {
            var courses = await _courseRepo.GetAllCoursesByDepartmentID(departmentId);
            return courses.Select(c => new EnrollCourseDTO
            {
                CreditHours = c.Credits,
                CourseName = c.Name,
                courseCode = c.CourseCode,
                DepartmentName = c.Department?.Name ?? "Unknown"
            });
        }

        public async Task<CreateEnrollmentDTO?> AddEnrollCourse(CreateEnrollmentDTO enrollCourseDto)
        {
            // Validate input
            if (enrollCourseDto.StudentId == 0)
                throw new ArgumentException("Student ID is required for enrollment.");

            if (enrollCourseDto.CourseId == 0)
                throw new ArgumentException("Course ID is required for enrollment.");

            // Check if student is already enrolled
            var existingEnrollment = await _courseRepo.GetEnrollmentByStudentIdAndCourseId(
                enrollCourseDto.StudentId,
                enrollCourseDto.CourseId);

            if (existingEnrollment != null)
                throw new InvalidOperationException("Student is already enrolled in this course.");

            var enrollment = new Enrollment
            {
                CourseId = enrollCourseDto.CourseId,
                StudentId = enrollCourseDto.StudentId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Enrolled"
            };

            await _courseRepo.AddEnrollCourse(enrollment);

            // Return enriched DTO
            return new CreateEnrollmentDTO
            {
                StudentId = enrollCourseDto.StudentId,
                CourseId = enrollCourseDto.CourseId,
                CourseCode = enrollCourseDto.CourseCode,
                CourseName = enrollCourseDto.CourseName,
                CreditHours = enrollCourseDto.CreditHours
            };
        }

        public async Task<bool> RemoveEnrollCourse(int enrollmentId)
        {
            // Fixed: was incorrectly using GetCourseById
            return await _courseRepo.RemoveEnrollCourse(enrollmentId);
        }

        public async Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _courseRepo.GetEnrollmentsByStudentId(studentId);
            return enrollments.Select(e => new studentEnrollmentDTO
            {
                courseName = e.Course?.Name ?? "Unknown",
                courseCode = e.Course?.CourseCode ?? "Unknown",
                studentName = e.Student?.FullName ?? "Unknown",
                CreditHours = e.Course?.Credits ?? 0,
                DepartmentName = e.Course?.Department?.Name ?? "Unknown"
            });
        }

        public async Task<IEnumerable<InstructorCoursesDTO>> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _courseRepo.GetCoursesByInstructorId(instructorId);
            return courses.Select(c => new InstructorCoursesDTO
            {
                CourseName = c.Name,
                CourseCode = c.CourseCode,
                DepartmentName = c.Department?.Name ?? "Unknown",
                CreditHours = c.Credits,
                InstructorName = c.Instructor?.FullName ?? "Unknown"
            });
        }
    }
}